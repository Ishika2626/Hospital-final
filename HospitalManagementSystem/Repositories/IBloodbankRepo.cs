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
    }
}
