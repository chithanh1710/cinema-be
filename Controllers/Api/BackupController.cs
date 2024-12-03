using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CINEMA_BE.Controllers.Api
{
    public class BackupController : ApiController
    {
        QL_RCP_Entities db = new QL_RCP_Entities();

        // Connection string từ DbContext
        private readonly string connectionString;

        public BackupController()
        {
            // Lấy connection string từ DbContext
            connectionString = db.Database.Connection.ConnectionString;
        }

        // Model để gửi yêu cầu backup
        public class BackupRequest
        {
            public string BackupFileName { get; set; }
        }

        // Model để trả về lịch sử backup
        public class BackupHistory
        {
            public string DatabaseName { get; set; }
            public string BackupType { get; set; }
            public string BackupStartDate { get; set; }
            public string BackupFinishDate { get; set; }
            public string BackupPath { get; set; }
        }

        public class FileListResponse
        {
            public List<FileInfoResponse> Files { get; set; }
        }

        public class FileInfoResponse
        {
            public string FileName { get; set; }
            public string CreationTime { get; set; }
        }

        // Model yêu cầu khôi phục
        public class RestoreRequest
        {
            public string RestoreFileName { get; set; }
        }

        [HttpPost]
        [Route("api/backup/restore")]
        public IHttpActionResult RestoreDatabase([FromBody] RestoreRequest request)
        {
            try
            {
                // Tên file backup từ yêu cầu
                string fileName = request.RestoreFileName;
                if (string.IsNullOrEmpty(fileName))
                {
                    return BadRequest("Tên file backup không được để trống.");
                }

                // Đường dẫn thư mục backup cố định
                string backupDirectory = @"D:\Backup";

                // Đường dẫn đầy đủ tới file backup
                string backupPath = Path.Combine(backupDirectory, fileName + ".bak");

                // Kiểm tra nếu file không tồn tại
                if (!System.IO.File.Exists(backupPath))
                {
                    return BadRequest($"File backup không tồn tại tại: {backupPath}");
                }

                // Tên cơ sở dữ liệu cần khôi phục
                string databaseName = db.Database.Connection.Database;

                // Lệnh T-SQL để khôi phục cơ sở dữ liệu
                string commandText = $@"
                            USE master; 
                            ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                            RESTORE DATABASE [{databaseName}]
                            FROM DISK = '{backupPath}'
                            WITH REPLACE;
                            ALTER DATABASE [{databaseName}] SET MULTI_USER;";

                // Kết nối và thực thi lệnh SQL
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Thay đổi kết nối sang database 'master'
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("USE master;", connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    // Thực thi lệnh khôi phục
                    using (SqlCommand command = new SqlCommand(commandText, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                return Ok($"Khôi phục cơ sở dữ liệu '{databaseName}' từ file '{backupPath}' thành công!");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }



        public IHttpActionResult CreateBackup([FromBody] BackupRequest request)
        {
            try
            {
                // Tên file backup từ yêu cầu
                string fileName = request.BackupFileName;
                if (string.IsNullOrEmpty(fileName))
                {
                    return BadRequest("Tên file backup không được để trống.");
                }

                // Đường dẫn thư mục backup cố định
                string backupDirectory = @"D:\Backup";

                // Tạo thư mục nếu chưa tồn tại
                if (!System.IO.Directory.Exists(backupDirectory))
                {
                    System.IO.Directory.CreateDirectory(backupDirectory);
                }

                // Đường dẫn đầy đủ tới file backup
                string backupPath = Path.Combine(backupDirectory, fileName + ".bak");

                // Kiểm tra nếu file đã tồn tại
                if (System.IO.File.Exists(backupPath))
                {
                    return BadRequest($"File backup với tên '{fileName}.bak' đã tồn tại. Vui lòng chọn tên khác.");
                }

                // Tên cơ sở dữ liệu
                string databaseName = db.Database.Connection.Database;

                // Lệnh T-SQL để backup
                string commandText = $@"
            BACKUP DATABASE [{databaseName}]
            TO DISK = '{backupPath}'
            WITH FORMAT, INIT, NAME = 'Full Backup of {databaseName}'";

                // Kết nối và thực thi lệnh SQL
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(commandText, connection);
                    connection.Open();
                    command.ExecuteNonQuery();
                }

                return Ok($"Backup cơ sở dữ liệu '{databaseName}' thành công! File backup: {backupPath}");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/backup/history")]
        public IHttpActionResult GetHistory()
        {
            try
            {
                // Danh sách lịch sử backup
                List<BackupHistory> backupHistories = new List<BackupHistory>();

                // Truy vấn lịch sử backup từ msdb
                string query = @"
                    SELECT 
                        bs.database_name AS DatabaseName,
                        CASE bs.type 
                            WHEN 'D' THEN 'Full'
                            WHEN 'I' THEN 'Differential'
                            WHEN 'L' THEN 'Transaction Log'
                            ELSE 'Unknown' 
                        END AS BackupType,
                        bs.backup_start_date AS BackupStartDate,
                        bs.backup_finish_date AS BackupFinishDate,
                        bmf.physical_device_name AS BackupPath
                    FROM msdb.dbo.backupset bs
                    INNER JOIN msdb.dbo.backupmediafamily bmf ON bs.media_set_id = bmf.media_set_id
                    WHERE bs.database_name = 'QuanLy_RapChieuPhim'
                    ORDER BY bs.backup_start_date DESC";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            backupHistories.Add(new BackupHistory
                            {
                                DatabaseName = reader["DatabaseName"].ToString(),
                                BackupType = reader["BackupType"].ToString(),
                                BackupStartDate = reader["BackupStartDate"] != DBNull.Value
                                    ? Convert.ToDateTime(reader["BackupStartDate"]).ToString("yyyy-MM-dd HH:mm:ss")
                                    : null,
                                BackupFinishDate = reader["BackupFinishDate"] != DBNull.Value
                                    ? Convert.ToDateTime(reader["BackupFinishDate"]).ToString("yyyy-MM-dd HH:mm:ss")
                                    : null,
                                BackupPath = reader["BackupPath"].ToString()
                            });
                        }
                    }
                }

                return Ok(backupHistories);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public IHttpActionResult GET()
        {
            try
            {
                // Đường dẫn thư mục chứa các file backup
                string backupFolderPath = @"D:\Backup";

                // Kiểm tra thư mục có tồn tại không
                if (!Directory.Exists(backupFolderPath))
                {
                    return NotFound(); // Nếu thư mục không tồn tại
                }

                var backupFiles = Directory.GetFiles(backupFolderPath, "*.bak")
                                            .Select(f => new FileInfoResponse
                                            {
                                                FileName = Path.GetFileName(f), // Lấy tên file
                                                CreationTime = File.GetCreationTime(f).ToString("yyyy-MM-dd HH:mm:ss") // Định dạng thời gian
                                            })
                                            .ToList();

                // Trả về danh sách file dưới dạng JSON
                return Ok(new FileListResponse { Files = backupFiles });

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
