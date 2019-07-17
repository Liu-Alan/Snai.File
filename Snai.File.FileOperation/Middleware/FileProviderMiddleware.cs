using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Snai.File.FileOperation.Middleware.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snai.File.FileOperation.Middleware
{
    public class FileProviderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IFileProvider _fileProvider;

        public FileProviderMiddleware(RequestDelegate next, IFileProvider fileProvider)
        {
            _next = next;
            _fileProvider = fileProvider;
        }

        public async Task Invoke(HttpContext context)
        {
            var output = new StringBuilder("");
            //ResolveDirectory(output, "", "");
            ResolveFileInfo(output, "log", ".log");

            await context.Response.WriteAsync(output.ToString());
        }

        //读取目录下所有文件内容
        private void ResolveFileInfo(StringBuilder output, string path, string suffix)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            Encoding encoding = Encoding.GetEncoding("gb2312");

            output.AppendLine("UserID    Golds    RecordDate");

            IDirectoryContents dir = _fileProvider.GetDirectoryContents(path);
            foreach (IFileInfo item in dir)
            {
                if (item.IsDirectory)
                {
                    ResolveFileInfo(output,
                        item.PhysicalPath.Substring(Directory.GetCurrentDirectory().Length),
                        suffix);
                }
                else
                {
                    if (item.Name.Contains(suffix))
                    {
                        var userList = new List<UserGolds>();
                        var user = new UserGolds();

                        IFileInfo file = _fileProvider.GetFileInfo(path + "\\" + item.Name);

                        using (var stream = file.CreateReadStream())
                        {
                            using (var reader = new StreamReader(stream, encoding))
                            {
                                string content = reader.ReadLine();

                                while (content != null)
                                {
                                    if (content.Contains("begin"))
                                    {
                                        user = new UserGolds();
                                    }
                                    if (content.Contains("写入时间"))
                                    {
                                        DateTime recordDate;
                                        string strRecordDate = content.Substring(content.IndexOf(":") + 1).Trim();
                                        if (DateTime.TryParse(strRecordDate, out recordDate))
                                        {
                                            user.RecordDate = recordDate;
                                        }
                                    }

                                    if (content.Contains("userid"))
                                    {
                                        int userID;
                                        string strUserID = content.Substring(content.LastIndexOf("=") + 1).Trim();
                                        if (int.TryParse(strUserID, out userID))
                                        {
                                            user.UserID = userID;
                                        }
                                    }

                                    if (content.Contains("golds"))
                                    {
                                        int golds;
                                        string strGolds = content.Substring(content.LastIndexOf("=") + 1).Trim();
                                        if (int.TryParse(strGolds, out golds))
                                        {
                                            user.Golds = golds;
                                        }
                                    }

                                    if (content.Contains("end"))
                                    {
                                        var userMax = userList.FirstOrDefault(u => u.UserID == user.UserID);
                                        if (userMax == null || userMax.UserID <= 0)
                                        {
                                            userList.Add(user);
                                        }
                                        else if (userMax.RecordDate < user.RecordDate)
                                        {
                                            userList.Remove(userMax);
                                            userList.Add(user);
                                        }
                                    }

                                    content = reader.ReadLine();
                                }
                            }
                        }

                        if (userList != null && userList.Count > 0)
                        {
                            foreach (var golds in userList.OrderBy(u => u.RecordDate))
                            {
                                output.AppendLine(golds.UserID.ToString() + "    " + golds.Golds + "    " + golds.RecordDate);
                            }

                            output.AppendLine("");
                        }
                    }
                }
            }
        }

        //读取目录下所有文件名
        private void ResolveDirectory(StringBuilder output, string path, string prefix)
        {
            IDirectoryContents dir = _fileProvider.GetDirectoryContents(path);
            foreach (IFileInfo item in dir)
            {
                if (item.IsDirectory)
                {
                    output.AppendLine(prefix + "[" + item.Name + "]");

                    ResolveDirectory(output,
                        item.PhysicalPath.Substring(Directory.GetCurrentDirectory().Length),
                        prefix + "    ");
                }
                else
                {
                    output.AppendLine(path + prefix + item.Name);
                }
            }
        }
    }

    public static class UseFileProviderExtensions
    {
        public static IApplicationBuilder UseFileProvider(this IApplicationBuilder app)
        {
            return app.UseMiddleware<FileProviderMiddleware>();
        }
    }
}
