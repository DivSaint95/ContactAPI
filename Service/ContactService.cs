using ContactAPI.Common.Model;
using ContactAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Text.Json;

namespace ContactAPI.Service
{
    public class ContactService
    {
        private readonly string _filePath;

        public ContactService(IOptions<FileSetting> options) {
            _filePath = options.Value.FilePath;
        }
        #region contact list data
        public List<ContactModel> GetContactList()
        {
            if (!File.Exists(_filePath))
            {
                return new List<ContactModel>();
            }

            var jsonData = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<ContactModel>>(jsonData) ?? new List<ContactModel>();
        }
        #endregion

        #region contact list data by id
        public ContactModel GetById(int id)
        {
            var Contact = GetContactList();
            return Contact.FirstOrDefault(p => p.Id == id);
        }
        #endregion

        #region Add contact Details
        public void Add(ContactModel contactModel)
        {
            var contact = GetContactList();
            contactModel.Id = contact.Count == 0 ? 1 : contact.Max(p => p.Id) + 1;
            contact.Add(contactModel);
            SaveToFile(contact);
        }
        #endregion

        #region Update contact Details
        public void Update(ContactModel contactModel)
        {
            var contact = GetContactList();
            var index = contact.FindIndex(p => p.Id == contactModel.Id);
            if (index >= 0)
            {
                contact[index] = contactModel;
                SaveToFile(contact);
            }
        }

        #endregion

        #region Delete contact Details
        public void Delete(int id)
        {
            var contactList = GetContactList();
            var contact = contactList.FirstOrDefault(p => p.Id == id);
            if (contact != null)
            {
                contactList.Remove(contact);
                SaveToFile(contactList);
            }
        }
        #endregion


        #region save into jsom
        private void SaveToFile(List<ContactModel> contactModel)
        {
            var jsonData = JsonSerializer.Serialize(contactModel);
            File.WriteAllText(_filePath, jsonData);
        }
        #endregion
    }
}
