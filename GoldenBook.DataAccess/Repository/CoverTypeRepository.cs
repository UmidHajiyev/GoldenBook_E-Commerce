using GoldenBook.DataAccess.Repository.IRepository;
using GoldenBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBook.DataAccess.Repository
{
    public class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
    {
        private readonly ApplicationDbContext _db;
        public CoverTypeRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }

        public void Update(CoverType obj)
        {
            _db.Update(obj);
        }
    }
}
