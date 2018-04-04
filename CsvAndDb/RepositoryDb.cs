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
            using (IMDWHEntities dbContext = new IMDWHEntities())
            {
                List<Time> toReturn = dbContext.Time.Where(t => t.Date >= start && t.Date <= end && t.BankDay == true).OrderBy(t => t.Date).ToList(); ;
                return toReturn;
            }
        }

        public DateTime GetWorkingDateAfter(DateTime date)
        {
            List<Time> times = GetWorkingDaysBetween(date.AddDays(-10), date.AddDays(10));
            return GetWorkingDateAfter(date, times);
        }

        public DateTime GetWorkingDateAfter(DateTime date, List<Time> times)
        {
            DateTime toReturn;

            toReturn = times.FirstOrDefault(x => x.Date > date).Date.Value;
            return toReturn;
        }

        public DateTime GetWorkingDateBefore(DateTime date)
        {
            List<Time> times = GetWorkingDaysBetween(date.AddDays(-10), date.AddDays(10)).OrderByDescending(t => t.Date).ToList();
            return GetWorkingDateBefore(date, times);
        }

        public DateTime GetWorkingDateBefore(DateTime date, List<Time> times)
        {
            DateTime toReturn;

            toReturn = times.FirstOrDefault(x => x.Date < date).Date.Value;
            return toReturn;
        }
    }
}
