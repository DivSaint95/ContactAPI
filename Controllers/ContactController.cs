using ContactAPI.Model;
using ContactAPI.Service;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ContactAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly ContactService _contacService;

        public ContactController(ContactService contactService)
        {
            _contacService = contactService;
        }

        #region contact list data
        [HttpGet]
        public ActionResult<List<ContactModel>> Get()
        {
            return _contacService.GetContactList();
        }
        #endregion

        #region contact list data by id
        [HttpGet("{id}")]
        public ActionResult<ContactModel> Get(int id)
        {
            var person = _contacService.GetById(id);
            if (person == null)
            {
                return NotFound();
            }
            return person;
        }
        #endregion

        #region Add contact Details
        [HttpPost]
        public IActionResult Post([FromBody] ContactModel contactModel)
        {
            _contacService.Add(contactModel);
            return CreatedAtAction(nameof(Get), new { id = contactModel.Id }, contactModel);
        }
        #endregion

        #region Update contact Details
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] ContactModel contactModel)
        {
            var ContactUpdate = _contacService.GetById(id);
            if (ContactUpdate == null)
            {
                return NotFound();
            }

            contactModel.Id = id;
            _contacService.Update(contactModel);
            return NoContent();
        }

        #endregion

        #region Delete contact Details
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var contactDelete = _contacService.GetById(id);
            if (contactDelete == null)
            {
                return NotFound();
            }
            _contacService.Delete(id);
            return NoContent();
        }
        #endregion
    }
}
