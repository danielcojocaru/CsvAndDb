using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvAndDb
{
    public class RepositoryDb
    {
        public RepositoryDb()
        {

        }

        public List<Time> GetWorkingDaysBetween(DateTime start, DateTime end)
        {
            using (IMDWHEntities context = new IMDWHEntities())
            {
                List<Time> toReturn = context.Time.Where(t => t.Date >= start && t.Date <= end && t.BankDay == true).OrderBy(t => t.Date).ToList(); ;
                return toReturn;
            }
        }
    }
}
