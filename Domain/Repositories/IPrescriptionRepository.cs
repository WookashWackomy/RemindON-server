using RemindONServer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RemindONServer.Domain.Repositories
{
    public interface IPrescriptionRepository : IRepository<Prescription>
    {
    }
}
