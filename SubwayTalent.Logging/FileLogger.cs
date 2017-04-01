using SubwayTalent.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubwayTalent.Logging
{
    public class FileLogger : ILogger
    {
        private string _path = "";

        public void Log(string logMessage)
        {
            try
            {
                _path = ResolveFilePath();

                if (!Directory.Exists(_path))
                    Directory.CreateDirectory((new FileInfo(_path)).DirectoryName);

                using (FileStream fileStream = new FileStream(_path, FileMode.Append))
                {
                    using (StreamWriter writer = new StreamWriter(fileStream))
                    {
                        writer.WriteLine(string.Format("[{0}] -----------------------------------------------", DateTime.Now.ToLongTimeString()));
                        writer.WriteLine(logMessage);
                        writer.WriteLine();
                        writer.WriteLine("------------------------------------------------------------");
                    }
                }
            }
            catch (Exception)
            {
                // do nothing
            }

        }

        private string ResolveFilePath()
        {

            return AppDomain.CurrentDomain.BaseDirectory + string.Format(@"\Logs\/{0}_{1}{2}{3}.log", "LogFile", DateTime.Now.Month.ToString(), DateTime.Now.Day, DateTime.Now.Year);


        }
    }


}
