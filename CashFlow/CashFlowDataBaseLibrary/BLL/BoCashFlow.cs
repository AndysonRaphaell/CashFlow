using CashFlowDataBaseLibrary.DAL;
using CashFlowDataBaseLibrary.DML;
using CashFlowDataBaseLibrary.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashFlowDataBaseLibrary.BLL
{
    public class BoCashFlow
    {
        private CashFlowDBContext _dbContext;

        public BoCashFlow()
        {
            string dataSourcePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "CashFlow");
            if (!Directory.Exists(dataSourcePath))
                Directory.CreateDirectory(dataSourcePath);

            dataSourcePath = Path.Combine(dataSourcePath, "CashFlowDb.db");
            _dbContext = new CashFlowDBContext(dataSourcePath);
            CreateDataBase();
        }

        private async void CreateDataBase()
        {
            await _dbContext.Database.EnsureCreatedAsync();
        }

        public CashFlowDataSet LoadBalanceInformations()
        {
            try
            {
                if (_dbContext.CashFlowDbSet.Any())
                {
                    CashFlowDataSet lastBalance = _dbContext.CashFlowDbSet.OrderBy(c => c.Id).LastOrDefault();
                    return lastBalance;
                }

                return new CashFlowDataSet();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task InsertCashReleased(double value, TypesEnum.CashReleaseType releaseType, double previousValue, double totalBalance)
        {
            try
            {
                CashFlowDataSet newDataSet = new CashFlowDataSet()
                {
                    ReleasedValue = value,
                    PreviousValue = previousValue,
                    BalanceValue = totalBalance,
                    ReleaseType = releaseType,
                    ReleaseData = DateTime.Now
                };

                _dbContext.CashFlowDbSet.Add(newDataSet);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetBalanceByDate(DateTime initialDate, DateTime finalDate)
        {
            try
            {
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"Report_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
                if (_dbContext.CashFlowDbSet.Any())
                {
                    var lastBalance = _dbContext.CashFlowDbSet.OrderBy(c => c.Id);

                    List<string> report = new List<string>();
                    report.Add(string.Join(';', new string[] { "Release Date", "Cash", "Cash Type" }));

                    int totalCredits = 0;
                    int totalDebts = 0;
                    double valueTotalCredits = 0d;
                    double valueTotalDebts = 0d;
                    foreach (CashFlowDataSet item in lastBalance)
                    {
                        if (item.ReleaseData.Date < initialDate.Date || item.ReleaseData.Date > finalDate.Date)
                            continue;

                        string dateTime = item.ReleaseData.ToString("dd/MM/yyyy HH:mm:ss");
                        string cash = item.ReleasedValue.ToString("N2");
                        string cashType = item.ReleaseType == TypesEnum.CashReleaseType.Credit ? "Credit" : "Debit";
                        totalCredits += item.ReleaseType == TypesEnum.CashReleaseType.Credit ? 1 : 0;
                        totalDebts += item.ReleaseType == TypesEnum.CashReleaseType.Credit ? 0 : 1;
                        valueTotalCredits += item.ReleaseType == TypesEnum.CashReleaseType.Credit ? item.ReleasedValue : 0d;
                        valueTotalDebts += item.ReleaseType == TypesEnum.CashReleaseType.Credit ? 0d : item.ReleasedValue;

                        report.Add(string.Join(';', new string[] { dateTime, cash, cashType }));
                    }

                    report.Add(Environment.NewLine);
                    report.Add($"Total credits in the period: {totalCredits}");
                    report.Add($"Total debts in the period: {totalDebts}");
                    report.Add($"Total balance of credits: R$ {valueTotalCredits:N2}");
                    report.Add($"Total balance of debts: R$ {valueTotalDebts:N2}");
                    report.Add($"Total balance in the period: R$ {(valueTotalCredits - valueTotalDebts):N2}");

                    File.WriteAllLines(filePath, report);
                }

                return filePath;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
