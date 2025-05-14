using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.Repositories
{
    public interface IStaffRepository
    {
        // Role CRUD
        IEnumerable<Role> GetAllRoles();
        Role GetRoleById(int id);
        void AddRole(Role role);
        void UpdateRole(Role role);
        void DeleteRole(int id);
    
        //Department CRUD
        List<Department> GetAllDepartments();
        Department GetDepartmentById(int departmentId);
        void AddDepartment(Department department);
        void UpdateDepartment(Department department);
        void DeleteDepartment(int departmentId);

        //Employee CRUD
        IEnumerable<Employee> GetAll();
        Employee GetById(int id);
        void Add(Employee employee);
        void Update(Employee employee);
        void Delete(int id);
        Employee Login(string loginId, string password);
        void Updateprofile(Employee model);
        bool ChangePassword(int employeeId, string currentPassword, string newPassword);
        Employee GetByLoginIdOrEmail(string input);
        bool UpdatePassword(string loginId, string newPassword);

        //nurse related
        List<PatientRegistration> GetAssignedPatients(int nurseId);
        List<PatientStatusReport> GetPatientStatusReport(int nurseId);
        void SavePatientVitals(PatientVitals vitals);
        List<PatientVitalsReport> GetVitalsReport();
        List<DischargedPatientViewModel> GetDischargedPatientsByNurse(int nurseId);
        void UpdateDischargeDetails(int admissionId, DateTime dischargeDate, string dischargeReason, int? doctorId);
        List<Doctor> GetAllDoctors();


        //HR Manager Related

        List<HiringManagement> GetAllJobs();
        HiringManagement GetJobById(int jobId);
        void AddJob(HiringManagement job);
        void UpdateJob(HiringManagement job);
        void DeleteJob(int jobId);
        void CloseJob(int jobId);
        List<HiringManagement> GetAllOpenJobs();

        void UpdateVacancies(int jobId, int vacancies);

        IEnumerable<SchedulingModel> GetAllSchedules();
        SchedulingModel GetScheduleById(int id);
        void InsertSchedule(SchedulingModel schedule);
        void UpdateSchedule(SchedulingModel schedule);
        void DeleteSchedule(int id);
        List<AttendanceModel> GetAllAttendance();
        //void MarkAttendance(AttendanceModel attendance);
        AttendanceModel GetAttendanceForToday(int employeeId, DateTime date);
        void InsertPunchIn(AttendanceModel model);
        void UpdatePunchOut(AttendanceModel model);

        void ApplyLeave(LeaveRequest request);
        IEnumerable<Employee> GetEmployee();
        List<LeaveRequest> GetAllLeaveRequests();
        void UpdateLeaveStatus(int requestId, string status);
        List<PerformanceReview> GetAllReviews();
        void AddReview(PerformanceReview review);
        List<EmployeeTraining> GetAllEmpTraining();
        List<Training> GetAllTrainings();
        EmployeeTraining GetTraningById(int id);
        void AddEmpTraining(EmployeeTraining training);
        void UpdateTrainingStatus(int trainingId, string trainingStatus);
        void DeleteEmpTraining(int id);  // Delete an employee training record
        IEnumerable<Payroll> GenerateMonthlyPayroll(DateTime payDate);

        Payroll GetSalarySlip(int employeeId, DateTime payDate);

        //Receptionist
        IEnumerable<Doctor> GetDoctorsByDepartment(int departmentId);           
    }
}
