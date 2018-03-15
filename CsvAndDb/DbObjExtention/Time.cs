using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvAndDb
{
    public partial class Time
    {
        public override string ToString()
        {
            return this.Date + " : " + this.BankDay;
        }
    }
}
