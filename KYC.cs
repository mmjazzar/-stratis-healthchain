using Stratis.SmartContracts;

[Deploy]
public class RegistrationContract : SmartContract
{
    public RegistrationContract(ISmartContractState state) : base(state)
    {
        this.Admin = Message.Sender;
    }

    public Address Admin
    {
        get => PersistentState.GetAddress(nameof(Admin));
        private set => PersistentState.SetAddress(nameof(Admin), value);
    }

    enum requestStatus : uint
    {
        Submited = 0,
        Approved = 1,
        Rejected = 2,
        Banned = 3
    }

    public struct Hospital
    {
        public Address cryptoAddress;
        public string  name;
        public string  email;
        public string  location;
    }

    public struct Doctor
    {
        public Address cryptoAddress;
        public uint status;
        public Address hospital;
        public string  name;
        public string  email;
    }

    public Hospital GetHospital(Address cryptoAddress)
    {
        return PersistentState.GetStruct<Hospital>($"Hospital:{cryptoAddress}");
    }

    private void SetHospital(Address cryptoAddress, Hospital hospital)
    {
        PersistentState.SetStruct($"Hospital:{cryptoAddress}", hospital);
    }

    public Doctor GetDoctor(Address cryptoAddress)
    {
        return PersistentState.GetStruct<Doctor>($"Doctor:{cryptoAddress}");
    }

    private void SetDoctor(Address cryptoAddress, Doctor doctor)
    {
        PersistentState.SetStruct($"Doctor:{cryptoAddress}", doctor);
    }

    public bool HospitalRegister(string name, string email, string location)
    {
        Hospital hospital;
        hospital.cryptoAddress = Message.Sender;
        hospital.name = name;
        hospital.email = email;
        hospital.location = location;
        // hospital.doctors = [];
        // hospital.patient = [];

        SetHospital(hospital.cryptoAddress, hospital);
        Log(hospital);
        return true;
    }

    public bool DoctorRegister(string name, string email, Address hospitalAddress)
    {
        Doctor doctor;
        doctor.cryptoAddress = Message.Sender;
        doctor.name = name;
        doctor.email = email;
        doctor.hospital = hospitalAddress;
        doctor.status = (uint)requestStatus.Submited;
        SetDoctor(doctor.cryptoAddress, doctor);
        Log(doctor);
        return true;
    }

    public bool HospitalHandleRegistration(Address doctorAddress, uint status)
    {
        var doctor = GetDoctor(doctorAddress);
        Assert(this.Message.Sender == doctor.hospital);
        Assert(doctor.status == (uint)requestStatus.Submited);
        Assert(status == (uint)requestStatus.Approved || status == (uint)requestStatus.Rejected || status == (uint)requestStatus.Banned);
        doctor.status = status;
        SetDoctor(doctorAddress, doctor);
        Log(new HospitalHandleRegistrationLog{doctor = doctor});
        return true;
    }

    public struct HospitalHandleRegistrationLog
    {
        public Doctor doctor;
    }

}
