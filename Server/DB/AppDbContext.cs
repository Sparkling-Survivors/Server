using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Server.DB
{
    // EF Core 작동 스탭
    // 1. DBContext 만들때
    // 2. DbSet<T>을 찾는다
    // 3. 모델링 class 분석해서, 칼럼을 찾는다
    // 4. 모델링 class에서 참조하는 다른 class가 있으면, 걔도 분석한다
    // 5. OnModelCreating 함수 호출 (추가 설정 = override)
    // 6. 데이터베이스의 전체 모델링 구조를 내부 메모리에 들고 있음

    class AppDbContext : DbContext
    {
        public DbSet<Setting> Setting { get; set; }


        //어떤 DB를 어떻게 연결해라(각종 설정, Authorization 등)
        public const string ConnectionString = "Data Source=together-rds.ch0meg46eg60.ap-northeast-2.rds.amazonaws.com;Initial Catalog=TogetherDB;Persist Security Info=True;User ID=admin;Password=igh17172080;Encrypt=False;Trust Server Certificate=True";

        //db랑 연동하는 부분을 옵션으로 넣는 것
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
        }
    }
}
