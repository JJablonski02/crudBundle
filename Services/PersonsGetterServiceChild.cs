using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using RepositoryContracts;
using Serilog;
using ServiceContracts.DTO;

namespace Services
{
    public class PersonsGetterServiceChild : PersonsGetterService
    {
        public PersonsGetterServiceChild(IPersonsRepository personsRepository, ILogger<PersonsGetterService> logger, IDiagnosticContext diagnosticContext) : base (personsRepository, logger, diagnosticContext)
        {

        }
        public async override Task<MemoryStream> GetPersonsExcel()
        {
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets.Add("PersonsSheet");

                workSheet.Cells["A1"].Value = "Person Name";
                workSheet.Cells["B1"].Value = "Email";
                workSheet.Cells["C1"].Value = "Date of Birth";
                workSheet.Cells["D1"].Value = "Age";
                workSheet.Cells["E1"].Value = "Gender";
                workSheet.Cells["F1"].Value = "Country";
                workSheet.Cells["G1"].Value = "Address";
                workSheet.Cells["H1"].Value = "Receive News Letters";

                using (ExcelRange headerCells = workSheet.Cells["A1:H1"])
                {
                    headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkOliveGreen);
                    headerCells.Style.Font.Bold = true;
                }

                int row = 2;
                List<PersonResponse> persons = await GetAllPersons();
                foreach (PersonResponse person in persons)
                {
                    workSheet.Cells[row, 1].Value = person.PersonName;
                    workSheet.Cells[row, 2].Value = person.Email;
                    if (person.DateOfBirth.HasValue)
                    {
                        workSheet.Cells[row, 3].Value = person.DateOfBirth.Value.ToString("dd-MM-yyyy");
                    }
                    workSheet.Cells[row, 4].Value = person.Age;
                    workSheet.Cells[row, 5].Value = person.Gender;
                    workSheet.Cells[row, 6].Value = person.Country;
                    workSheet.Cells[row, 7].Value = person.Address;
                    workSheet.Cells[row, 8].Value = person.ReceiveNewsLetters;

                    row++;
                }

                workSheet.Cells[$"A1:H{row}"].AutoFitColumns();

                await excelPackage.SaveAsync();
            }

            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
