﻿using Microsoft.Extensions.Logging;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CommonNetCoreFuncs.Tools
{
    public class NpoiExportHelpers
    {
        private readonly ILogger<NpoiExportHelpers> logger;

        public NpoiExportHelpers(ILogger<NpoiExportHelpers> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// GenericExcelExport
        /// </summary>
        /// <param name="dataList"></param>
        /// <param name="memoryStream"></param>
        /// <param name="tempLocation"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">Ignore.</exception>
        /// <exception cref="IOException">Ignore.</exception>
        /// <exception cref="System.Security.SecurityException">Ignore.</exception>
        /// <exception cref="DirectoryNotFoundException">Ignore.</exception>
        /// <exception cref="UnauthorizedAccessException">Ignore.</exception>
        /// <exception cref="PathTooLongException">Ignore.</exception>
        /// <exception cref="ObjectDisposedException">Ignore.</exception>
        public async Task<MemoryStream> GenericExcelExport<T>(List<T> dataList, MemoryStream memoryStream)
        {
            try
            {
                XSSFWorkbook wb = new XSSFWorkbook();
                ISheet ws = wb.CreateSheet("Data");
                if (dataList != null)
                {
                    if (!NpoiCommonHelpers.ExportFromTable(wb, ws, dataList))
                    {
                        return null;
                    }
                }

                await memoryStream.WriteFileToMemoryStreamAsync(wb);
                
                return memoryStream;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "");
            }
            
            return new MemoryStream();
        }
    }
}