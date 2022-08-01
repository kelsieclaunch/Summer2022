using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Lab8Handout.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
//using Newtonsoft.Json;

namespace Lab8Handout.Controllers;


public class HomeController : Controller
{

    // WARNING:
    // This very simple web server is designed to be as tiny and simple as possible
    // This is NOT the way to save user data.
    // This will only allow one user of the web server at a time (aside from major security concerns).
    private static string user = "";
    private static int card = -1;

    private readonly LibraryContext db;
    public HomeController(LibraryContext _db)
    {
        db = _db;
    }

    /// <summary>
    /// Given a Patron name and CardNum, verify that they exist and match in the database.
    /// If the login is successful, sets the global variables "user" and "card"
    /// </summary>
    /// <param name="name">The Patron's name</param>
    /// <param name="cardnum">The Patron's card number</param>
    /// <returns>A JSON object with a single field: "success" with a boolean value:
    /// true if the login is accepted, false otherwise.
    /// </returns>
    [HttpPost]
    public IActionResult CheckLogin(string name, int cardnum)
    {
        // TODO: Fill in. Determine if login is successful or not.
        bool loginSuccessful = false;

        var nameQuery = from Patron in db.Patrons
                        where Patron.Name == name
                        select Patron.Name;

        string thisName = nameQuery.FirstOrDefault();

        var cardQuery = (from Patron in db.Patrons
                        where Patron.CardNum == cardnum
                        select Patron.CardNum).Take(1);

        uint thisCard = cardQuery.FirstOrDefault();
        
        


        if (thisName == name && thisCard == cardnum)
        {
            loginSuccessful = true;
        }

        //if Patron name exists and CardNum matches
        //loginSuccessful = true;


        if (!loginSuccessful)
        {
            return Json(new { success = false });
        }
        else
        {
            user = name;
            card = cardnum;
            return Json(new { success = true });
        }
    }


    /// <summary>
    /// Logs a user out. This is implemented for you.
    /// </summary>
    /// <returns>Success</returns>
    [HttpPost]
    public ActionResult LogOut()
    {
        user = "";
        card = -1;
        return Json(new { success = true });
    }

    /// <summary>
    /// Returns a JSON array representing all known books.
    /// Each book should contain the following fields:
    /// {"isbn" (string), "title" (string), "author" (string), "serial" (uint?), "name" (string)}
    /// Every object in the list should have isbn, title, and author.
    /// Books that are not in the Library's inventory (such as Dune) should have a null serial.
    /// The "name" field is the name of the Patron who currently has the book checked out (if any)
    /// Books that are not checked out should have an empty string "" for name.
    /// </summary>
    /// <returns>The JSON representation of the books</returns>
    [HttpPost]
    public ActionResult AllTitles()
    {

        var bookQuery =
           from Title in db.Titles.DefaultIfEmpty()
           join Inventory in db.Inventories
           on Title.Isbn equals Inventory.Isbn into table1

           from Inventory in table1.DefaultIfEmpty()
           join CheckedOut in db.CheckedOuts.DefaultIfEmpty()
           on Inventory.Serial equals CheckedOut.Serial into table2

           from CheckedOut in table2.DefaultIfEmpty()
           join Patron in db.Patrons.DefaultIfEmpty()
           on CheckedOut.CardNum equals Patron.CardNum into table3

           from Patron in table3.DefaultIfEmpty()
           select new 
           {
               isbn = Title.Isbn,
               title = Title.Title1,
               author = Title.Author,
               serial = Inventory == null ? 0 : (uint?)Inventory.Serial,
               name = Patron.Name == null ? "" : Patron.Name

           };

  

        var ret = Json(bookQuery.ToArray());

        return ret;
    }



    /// <summary>
    /// Returns a JSON array representing all books checked out by the logged in user 
    /// The logged in user is tracked by the global variable "card".
    /// Every object in the array should contain the following fields:
    /// {"title" (string), "author" (string), "serial" (uint) (note this is not a nullable uint) }
    /// Every object in the list should have a valid (non-null) value for each field.
    /// </summary>
    /// <returns>The JSON representation of the books</returns>
    [HttpPost]
    public ActionResult ListMyBooks()
    {

        var bookQuery =
            from Patron in db.Patrons.DefaultIfEmpty()
            where Patron.Name == user
            join CheckedOut in db.CheckedOuts.DefaultIfEmpty()
            on Patron.CardNum equals CheckedOut.CardNum into table1

          

            from CheckedOut in table1.DefaultIfEmpty()
            join Inventory in db.Inventories
            on CheckedOut.Serial equals Inventory.Serial into table2

            from Inventory in table2.DefaultIfEmpty()
            join Title in db.Titles
            on Inventory.Isbn equals Title.Isbn into table3

            from Title in table3
            select new
            {
                title = Title.Title1,
                author = Title.Author,
                serial = CheckedOut.Serial,
            };

  
        var ret = Json(bookQuery.ToArray());

        return ret;
    }


    /// <summary>
    /// Updates the database to represent that
    /// the given book is checked out by the logged in user (global variable "card").
    /// In other words, insert a row into the CheckedOut table.
    /// You can assume that the book is not currently checked out by anyone.
    /// </summary>
    /// <param name="serial">The serial number of the book to check out</param>
    /// <returns>success</returns>
    [HttpPost]
    public ActionResult CheckOutBook(int serial)
    {
        // You may have to cast serial to a (uint)
        CheckedOut book = new CheckedOut();
        book.Serial = (uint)serial;
        book.CardNum = (uint)card;

        db.CheckedOuts.Add(book);
        try
        {
            db.SaveChanges();

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        return Json(new { success = true });
    }


    /// <summary>
    /// Returns a book currently checked out by the logged in user (global variable "card").
    /// In other words, removes a row from the CheckedOut table.
    /// You can assume the book is checked out by the user.
    /// </summary>
    /// <param name="serial">The serial number of the book to return</param>
    /// <returns>Success</returns>
    [HttpPost]
    public ActionResult ReturnBook(int serial)
    {
        var toDelete = from CheckedOut in db.CheckedOuts
                       where CheckedOut.Serial == serial
                       select CheckedOut;
        foreach(var attribute in toDelete)
        {
            db.CheckedOuts.Remove(attribute);
        }

        try
        {
            db.SaveChanges();
        }
        catch(Exception e)
        {
            Console.WriteLine(e);
        }
        // You may have to cast serial to a (uint)

        return Json(new { success = true });
    }

    /*******************************************/
    /****** Do not modify below this line ******/
    /*******************************************/

    /// <summary>
    /// Return the home page.
    /// </summary>
    /// <returns></returns>
    public IActionResult Index()
    {
        if (user == "" && card == -1)
            return View("Login");

        return View();
    }

    /// <summary>
    /// Return the MyBooks page.
    /// </summary>
    /// <returns></returns>
    public IActionResult MyBooks()
    {
        if (user == "" && card == -1)
            return View("Login");

        return View();
    }

    /// <summary>
    /// Return the About page.
    /// </summary>
    /// <returns></returns>
    public IActionResult About()
    {
        ViewData["Message"] = "Your application description page.";

        return View();
    }

    /// <summary>
    /// Return the Login page.
    /// </summary>
    /// <returns></returns>
    public IActionResult Login()
    {
        user = "";
        card = -1;

        ViewData["Message"] = "Please login.";

        return View();
    }


    /// <summary>
    /// Return the Contact page.
    /// </summary>
    /// <returns></returns>
    public IActionResult Contact()
    {
        ViewData["Message"] = "Your contact page.";

        return View();
    }

    /// <summary>
    /// Return the Error page.
    /// </summary>
    /// <returns></returns>
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}




