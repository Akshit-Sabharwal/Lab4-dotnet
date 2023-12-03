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
using Azure.Storage.Blobs;
using Azure;
using Microsoft.IdentityModel.Tokens;
using System.IO;
using Azure.Storage.Blobs.Specialized;

namespace Lab4.Controllers
{
    public class NewsController : Controller
    {
        private readonly SportsDbContext _context;
        private readonly BlobServiceClient _blobServiceClient;
        public NewsController(SportsDbContext context, BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
            _context = context;
        }
        [BindProperty]
        public News News { get; set; }
        public async Task<IActionResult> Index(string id)
        {
            Console.WriteLine("Is this working");
            Console.WriteLine("scid:" + id);
            var viewModel = new NewsViewModel { SportClub = await _context.SportClubs.FindAsync(id), News = await _context.News.Where(n => n.SportsClubId == id).ToListAsync() };
            
            return View(viewModel);
        }

        // GET: News
     /*   public async Task<IActionResult> Index()
        {
            var sportsDbContext = _context.News.Include(n => n.SportClub);
            return View(await sportsDbContext.ToListAsync());
        }*/

        // GET: News/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.News == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .Include(n => n.SportClub)
                .FirstOrDefaultAsync(m => m.NewsId == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        public async Task<IActionResult> Create(string id)
        {
            var SportClub = await _context.SportClubs.FindAsync(id);
            string title = SportClub.Title;
            
            var viewModel = new FileInputViewModel { SportClubId = id, SportClubTitle = title };
            return View(viewModel);
        }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> Upload(string id, IFormFile file)
        {
            if(file== null)
            {
                return Redirect("/Home/Error"); ;
            }
 
            var SportClub = await _context.SportClubs.FindAsync(id);
            BlobContainerClient containerClient;
            Console.WriteLine(id);
            string containerName = SportClub.Title.ToLower();
            string[] permittedExtensions = { ".jpg", ".png" };
            Console.WriteLine(file.FileName);
            News.FileName = file.FileName;
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                //return error for wrong submissions
                return Redirect("/Home/Error"); 
            }


            try
            {
                containerClient = await _blobServiceClient.CreateBlobContainerAsync(containerName);
                // Give access to public
                containerClient.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
            }
            catch (RequestFailedException)
            {
                Console.WriteLine("This is now running, because container was created previously....");
                containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            }
             
                var blob = containerClient.GetBlobClient(News.FileName);
            if (await blob.ExistsAsync())
            {
                // Handle the case where the blob already exists
                return Redirect("/Home/Error"); 

            }
            using (var memoryStream = new MemoryStream())
            {
                // copy the file data into memory
                await file.CopyToAsync(memoryStream);

                // navigate back to the beginning of the memory stream
                memoryStream.Position = 0;

                // send the file to the cloud
                await blob.UploadAsync(memoryStream);
                memoryStream.Close();
            }
             
               
                News.Url = blob.Uri.ToString();
                News.SportClub = SportClub;
                News.SportsClubId = id;  
 
           _context.Add(News);  //create the prediction

                await _context.SaveChangesAsync();

          var viewModel = new NewsViewModel { SportClub = await _context.SportClubs.FindAsync(id), News = await _context.News.Where(n => n.SportsClubId == id).ToListAsync() };
            
            return RedirectToAction("Index", new {  id = id});
        }



// GET: News/Create
//     public IActionResult Create()
//    {
//        ViewData["SportsClubId"] = new SelectList(_context.SportClubs, "Id", "Id");
//        return View();
//    }

// POST: News/Create
// To protect from overposting attacks, enable the specific properties you want to bind to.
// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
/*     [HttpPost]
     [ValidateAntiForgeryToken]
     public async Task<IActionResult> Create([Bind("NewsId,FileName,Url,SportsClubId")] News news)
     {
         if (ModelState.IsValid)
         {
             _context.Add(news);
             await _context.SaveChangesAsync();
             return RedirectToAction(nameof(Index));
         }
         ViewData["SportsClubId"] = new SelectList(_context.SportClubs, "Id", "Id", news.SportsClubId);
         return View(news);
     }*/

// GET: News/Edit/5
public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.News == null)
            {
                return NotFound();
            }

            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return NotFound();
            }
            ViewData["SportsClubId"] = new SelectList(_context.SportClubs, "Id", "Id", news.SportsClubId);
            return View(news);
        }

        // POST: News/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NewsId,FileName,Url,SportsClubId")] News news)
        {
            if (id != news.NewsId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(news);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsExists(news.NewsId))
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
            ViewData["SportsClubId"] = new SelectList(_context.SportClubs, "Id", "Id", news.SportsClubId);
            return View(news);
        }

        // GET: News/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            Console.WriteLine("111");
            if (id == null || _context.News == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .Include(n => n.SportClub)
                .FirstOrDefaultAsync(m => m.NewsId == id);
            if (news == null)
            {
                return NotFound();
            }
            
            return View(news);
        }

        // POST: News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Console.WriteLine("222");
            if (_context.News == null)
            {
                return Problem("Entity set 'SportsDbContext.News'  is null.");
            }
            var news = await _context.News.FindAsync(id);
            string scId = news.SportsClubId;
        
            BlobContainerClient containerClient;

            if (news != null)
            {
                news.SportClub = await _context.SportClubs.FindAsync(scId);
                string containerName = news.SportClub.Title.ToLower();


                try
                {
                    containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                    containerClient.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
                }
                catch (RequestFailedException)
                {
                  
                    return View("Error");
                }
            
                var blob = containerClient.GetBlobClient(news.FileName);
                

                try
                {
                    await blob.DeleteIfExistsAsync();
                } catch (Exception ex) {
                    Console.WriteLine(news.FileName);

                }
               
               
                _context.News.Remove(news);


                await _context.SaveChangesAsync();
            }


            return RedirectToAction("Index", new { id = scId });
        }

        private bool NewsExists(int id)
        {
          return _context.News.Any(e => e.NewsId == id);
        }
    }
}
