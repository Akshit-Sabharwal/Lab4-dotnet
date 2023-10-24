using Lab4.Models;

namespace Lab4.Data
{
    public static class DbInitializer
    {
        public static void Initialize(SportsDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Fans.Any())
            {
                return;   // DB has been seeded
            }

            var fans = new Fan[]
            {
            new Fan{FirstName="Carson",LastName="Alexander",BirthDate=DateTime.Parse("1995-01-09")},
            new Fan{FirstName="Meredith",LastName="Alonso",BirthDate=DateTime.Parse("1992-09-05")},
            new Fan{FirstName="Arturo",LastName="Anand",BirthDate=DateTime.Parse("1993-10-09")},
            new Fan{FirstName="Gytis",LastName="Barzdukas",BirthDate=DateTime.Parse("1992-01-09")},
            };
            foreach (Fan s in fans)
            {
                context.Fans.Add(s);
            }
            context.SaveChanges();

            var sportClubs = new SportClub[]
            {
            new SportClub{Id="A1",Title="Alpha",Fee=300},
            new SportClub{Id="B1",Title="Beta",Fee=130},
            new SportClub{Id="O1",Title="Omega",Fee=390},
            };
            foreach (SportClub c in sportClubs)
            {
                context.SportClubs.Add(c);
            }
            context.SaveChanges();

            var subscriptions = new Subscription[]
            {
            new Subscription{FanId=1,SportClubId="A1"},
            new Subscription{FanId=1,SportClubId="B1"},
            new Subscription{FanId=1,SportClubId="O1"},
            new Subscription{FanId=2,SportClubId="A1"},
            new Subscription{FanId=2,SportClubId="B1"},
            new Subscription{FanId=3,SportClubId="A1"},
            };
            foreach (var subscription in subscriptions)
            {
                context.Subscriptions.Add(subscription);
            }
            context.SaveChanges();

        }
    }

}
