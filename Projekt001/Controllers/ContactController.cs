using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projekt001.Entities;
using Projekt001.Models;
using Projekt001.Repositories;

namespace Projekt001.Controllers

{
    public class ContactController : Controller
    {
        private readonly IContactFormRepository _contactRepository;
        private readonly ILogger<ContactController> _logger;

        public ContactController(IContactFormRepository contactRepository, ILogger<ContactController> logger)
        {
            _contactRepository = contactRepository;
            _logger = logger;

        }

        [HttpGet]
        public IActionResult Index()
        {
            // Display the empty contact form
            return View(new ContactModel
            {
                Name = string.Empty,
                Email = string.Empty,
                Message = string.Empty
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Protects against CSRF attacks
        public async Task<IActionResult> Index(ContactModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Map ViewModel to Entity
                    ContactEntity contactMessage = ContactEntity.FromModel(model);
                    // Add to repository
                    await _contactRepository.AddAsync(contactMessage);
                    // Save changes to the database
                    int changes = await _contactRepository.SaveChangesAsync();
                    if (changes > 0)
                    {
                        _logger.LogInformation("Successfully saved contact message from {Email}", model.Email);
                        TempData["SuccessMessage"] = "Twoje zapytanie zostało wysłane pomyślnie! Odpowiemy najszybciej jak będziemy mogli.";
                        return RedirectToAction("Confirmation");
                    }
                    else
                    {
                        _logger.LogWarning("Contact message from {Email} was processed but no changes were saved to the database.", model.Email);
                        ModelState.AddModelError(string.Empty, "Wiadomość nie została zapisana. Spróbuj ponownie.");
                    }
                }
                catch (DbUpdateException dbEx)
                {
                    _logger.LogError(dbEx, "Database update error while saving contact message from {Email}", model.Email);
                    ModelState.AddModelError(string.Empty, "Wystąpił błąd serwera podczas zapisywania wiadomości. Spróbuj ponownie później lub skontaktuj się z administratorem.");
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, "Generic error while saving contact message from {Email}", model.Email);
                    ModelState.AddModelError(string.Empty, "Wystąpił nieoczekiwany błąd. Spróbuj ponownie później.");
                }
            }
            else
            {
                _logger.LogWarning("Invalid model state for contact form submission from {Email}.", model.Email);
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Confirmation()
        {
            if (TempData["SuccessMessage"] == null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}