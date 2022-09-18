using CashFlowDataBaseLibrary.Utils;
using System;

namespace CashFlowDataBaseLibrary.DML
{
    public class CashFlowDataSet
    {
        public int Id { get; set; }

        public double ReleasedValue { get; set; }

        public double PreviousValue { get; set; }

        public double BalanceValue { get; set; }

        public TypesEnum.CashReleaseType ReleaseType { get; set; }

        public DateTime ReleaseData { get; set; }
    }
}
