using hospitalManagementSystem.Models;

namespace HospitalManagementSystem.Repositories
{
    public interface IBloodbankRepo
    {
        void AddDonor(Donor donor);
        void UpdateDonor(Donor donor);
        void DeleteDonor(int donorId);
        Donor GetDonorById(int donorId);
        List<Donor> GetAllDonors();
        void UpdateEligibility(int id, string eligibilityStatus);
        List<Donor> GetPagedDonors(int page, int pageSize, string search);
        int GetDonorCount(string search);
        void AddDonationWithBag(int donorId, string bloodType);
        IEnumerable<BloodDonationHistory> GetAllDonationHistory();
        IEnumerable<BloodBag> GetAllBloodBags();
        List<string> GetMatchingBloodTypes(string term);
        int GetTotalBloodBagsCountWithSearch(string searchTerm);
        IEnumerable<BloodBag> GetBloodBagsPagedWithSearch(string searchTerm, int page, int pageSize);
        List<Donor> GetEligibleDonorsByBloodType(string bloodType);
        void RecordDonation(int donorId, string bloodType);
    }
}
