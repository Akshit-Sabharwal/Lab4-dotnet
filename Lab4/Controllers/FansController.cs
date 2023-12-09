using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab4.Data;
using Lab4.Models;
using Lab4.Models.ViewModels;

namespace Lab4.Controllers
{
    public class FansController : Controller
    {
        private readonly SportsDbContext _context;

        public FansController(SportsDbContext context)
        {
            _context = context;
        }

        // GET: Fans
        public async Task<IActionResult> Index(int? ID)
        {
            var viewModel = new FanSportClubViewModel { Fans = await _context.Fans.ToListAsync() };

            if (ID != null)
            {
                Console.WriteLine(ID);
                var subscriptions = await _context.Subscriptions.ToListAsync();
                var sportsClubsID = (from s in subscriptions where s.FanId== ID select s.SportClubId).ToList();
                var sportsClub = await _context.SportClubs.ToListAsync();
                var subscribedSportClubs = (from s in sportsClub where sportsClubsID.Contains(s.Id) select s).ToList();

                viewModel.SportClubs = subscribedSportClubs.OrderBy(s=>s.Title);
            }
            return View(viewModel);
   
        }


        public async Task<IActionResult> EditSubscriptions(int ID)
        {
            if(ID == 0)
            {
                return Redirect("/Home/Error");
            }
            var viewModel = new FanSubscriptionViewModel { Fan = await _context.Fans.FindAsync(ID) };

            var subscriptions = await _context.Subscriptions.ToListAsync();
            var sportsClubsID = (from s in subscriptions where s.FanId == ID select s.SportClubId).ToList();
            var sportsClubs = await _context.SportClubs.ToListAsync();
            var subscribedSportClubs = (from s in sportsClubs where sportsClubsID.Contains(s.Id) select s).ToList();

            List <SportClubSubscriptionViewModel> sub = new List<SportClubSubscriptionViewModel>();

            foreach (var sc in sportsClubs)
            {
                if (subscribedSportClubs.Contains(sc))
                {
                    sub.Add(new SportClubSubscriptionViewModel
                    {
                        Title
                = sc.Title,
                        SportClubId = sc.Id,
                        IsMember = true
                    });
                } else
                {
                    sub.Add(new SportClubSubscriptionViewModel
                    {
                        Title
                = sc.Title,
                        SportClubId = sc.Id,
                        IsMember = false
                    });
                }
                
            }

            viewModel.Subscriptions = sub.OrderBy(s=>s.IsMember).ThenBy(s=>s.Title).ToList();

            return View(viewModel);

        }

        public async Task<IActionResult> Subscribe(int id1, string id2)
        {
            Console.WriteLine("ID1 :" + id1);
            Console.WriteLine("ID2 :" + id2);
            if (id1 == 0 || id2 == null)
            {
                return Redirect("/Home/Error");
            }
            var viewModel = new FanSubscriptionViewModel { Fan = await _context.Fans.FindAsync(id1) };

            var newSubscription = new Subscription { FanId = id1, SportClubId = id2 };

            _context.Subscriptions.Add(newSubscription);
            _context.SaveChanges();
            var sportsClubIDs = (from s in _context.Subscriptions where s.FanId == id1 select s.SportClubId).ToList();  //subscribed sports club id
            var sportsClubs = await _context.SportClubs.ToListAsync();  //full sports club list
            var subscribedSportClubs = (from s in sportsClubs where sportsClubIDs.Contains(s.Id) select s).ToList();  //subscribed sport clubs

            List<SportClubSubscriptionViewModel> sub = new List<SportClubSubscriptionViewModel>();

            foreach (var sc in sportsClubs)
            {
                if (subscribedSportClubs.Contains(sc))
                {
                    sub.Add(new SportClubSubscriptionViewModel
                    {
                        Title
                = sc.Title,
                        SportClubId = sc.Id,
                        IsMember = true
                    });
                }
                else
                {
                    sub.Add(new SportClubSubscriptionViewModel
                    {
                        Title
                = sc.Title,
                        SportClubId = sc.Id,
                        IsMember = false
                    });
                }

            }

            viewModel.Subscriptions = sub.OrderBy(s => s.IsMember).ThenBy(s => s.Title).ToList();

            return RedirectToAction("EditSubscriptions", new { id = id1 });

        }

        public async Task<IActionResult> Unsubscribe(int id1, string id2)
        {
            Console.WriteLine("ID1 :" + id1);
            Console.WriteLine("ID2 :" + id2);
            if (id1== 0 || id2==null)
            {
                return Redirect("/Home/Error");
            }
            var viewModel = new FanSubscriptionViewModel { Fan = await _context.Fans.FindAsync(id1) };

             var removeSubscription = await _context.Subscriptions.FindAsync(id1,id2);

            _context.Subscriptions.Remove(removeSubscription);
            _context.SaveChanges();
            var sportsClubIDs = (from s in _context.Subscriptions where s.FanId ==id1 select s.SportClubId).ToList();  //subscribed sports club id
            var sportsClubs = await _context.SportClubs.ToListAsync();  //full sports club list
            var subscribedSportClubs = (from s in sportsClubs where sportsClubIDs.Contains(s.Id) select s).ToList();  //subscribed sport clubs

            List<SportClubSubscriptionViewModel> sub = new List<SportClubSubscriptionViewModel>();

            foreach (var sc in sportsClubs)
            {
                if (subscribedSportClubs.Contains(sc))
                {
                    sub.Add(new SportClubSubscriptionViewModel
                    {
                        Title
                = sc.Title,
                        SportClubId = sc.Id,
                        IsMember = true
                    });
                }
                else
                {
                    sub.Add(new SportClubSubscriptionViewModel
                    {
                        Title
                = sc.Title,
                        SportClubId = sc.Id,
                        IsMember = false
                    });
                }

            }

            viewModel.Subscriptions = sub.OrderBy(s => s.IsMember).ThenBy(s => s.Title).ToList();

            return RedirectToAction("EditSubscriptions",new {id=id1});

        }

       
        // GET: Fans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Fans == null)
            {
                return NotFound();
            }

            var fan = await _context.Fans
                .FirstOrDefaultAsync(m => m.ID == id);
            if (fan == null)
            {
                return NotFound();
            }

            return View(fan);
        }

        // GET: Fans/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Fans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,LastName,FirstName,BirthDate")] Fan fan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fan);
        }

        // GET: Fans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Fans == null)
            {
                return NotFound();
            }

            var fan = await _context.Fans.FindAsync(id);
            if (fan == null)
            {
                return NotFound();
            }
            return View(fan);
        }

        // POST: Fans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,LastName,FirstName,BirthDate")] Fan fan)
        {
            if (id != fan.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FanExists(fan.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(fan);
        }

        // GET: Fans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Fans == null)
            {
                return NotFound();
            }

            var fan = await _context.Fans
                .FirstOrDefaultAsync(m => m.ID == id);
            if (fan == null)
            {
                return NotFound();
            }

            return View(fan);
        }

        // POST: Fans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Fans == null)
            {
                return Problem("Entity set 'SportsDbContext.Fans'  is null.");
            }
            var fan = await _context.Fans.FindAsync(id);
            if (fan != null)
            {
                _context.Fans.Remove(fan);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FanExists(int id)
        {
          return _context.Fans.Any(e => e.ID == id);
        }
    }
}
