using Stratis.SmartContracts;

[Deploy]
public class RegistrationContract : SmartContract
{
    public RegistrationContract(ISmartContractState state) : base(state)
    {
        this.Admin = Message.Sender;
        this.NumberOfHospitals = 0;
    }

    public Address Admin
    {
        get => PersistentState.GetAddress(nameof(Admin));
        private set => PersistentState.SetAddress(nameof(Admin), value);
    }

    public uint NumberOfHospitals
    {
        get => PersistentState.GetUInt32(nameof(NumberOfHospitals));
        private set => PersistentState.SetUInt32(nameof(NumberOfHospitals), value);
    }

    public bool HospitalRegister(string name, string email, string location)
    {
        var createResult = Create<HospitalContract>(0, new object[] { this.Message.Sender, name, email, location});
        Assert(createResult.Success);
        PersistentState.SetBool("IsContract", PersistentState.IsContract(createResult.NewContractAddress));
        NumberOfHospitals ++;
        Log(new HospitalRegisterLog{cryptoAddress = this.Message.Sender, hospitalContract = createResult.NewContractAddress});
        return true;
    }

    public struct HospitalRegisterLog
    {
        [Index]
        public Address cryptoAddress;
        [Index]
        public Address hospitalContract;
    }

}


public class HospitalContract : SmartContract
{
    public HospitalContract(ISmartContractState state, Address owner, string name, string email, string location) : base(state)
    {
        this.Owner = owner;
        this.Name = name;
        this.Email = email;
        this.Location = location;
        this.NumberOfDoctors = 0;
    }

    public Address Owner
    {
        get => PersistentState.GetAddress(nameof(Owner));
        private set => PersistentState.SetAddress(nameof(Owner), value);
    }

    public uint NumberOfDoctors
    {
        get => PersistentState.GetUInt32(nameof(NumberOfDoctors));
        private set => PersistentState.SetUInt32(nameof(NumberOfDoctors), value);
    }

    public bool DoctorRegister(string name, string email)
    {
        var createResult = Create<DoctorContract>(0, new object[] { this.Message.Sender, this.Owner, name, email});
        Assert(createResult.Success);
        PersistentState.SetBool("IsContract", PersistentState.IsContract(createResult.NewContractAddress));
        this.NumberOfDoctors ++;
        Log(new DoctorRegisterLog{cryptoAddress = this.Message.Sender, doctorContract = createResult.NewContractAddress});
        return true;
    }

    public struct DoctorRegisterLog
    {
        [Index]
        public Address cryptoAddress;
        [Index]
        public Address doctorContract;
    }

    public string Name
    {
        get => PersistentState.GetString(nameof(Name));
        private set => PersistentState.SetString(nameof(Name), value);
    }

    public string Email
    {
        get => PersistentState.GetString(nameof(Email));
        private set => PersistentState.SetString(nameof(Email), value);
    }

    public string Location
    {
        get => PersistentState.GetString(nameof(Location));
        private set => PersistentState.SetString(nameof(Location), value);
    }

}


public class DoctorContract : SmartContract
{
    public DoctorContract(ISmartContractState state, Address owner, Address hospitalAddress, string name, string email) : base(state)
    {
        this.HospitalAddress = hospitalAddress;
        this.Owner = owner;
        this.Name = name;
        this.Email = email;
        State = (uint)StatusType.Submited;
    }

    enum StatusType : uint
    {
        Submited = 0,
        Approved = 1,
        Rejected = 2,
        Banned = 3
    }

    public uint State
    {
        get => PersistentState.GetUInt32(nameof(State));
        private set => PersistentState.SetUInt32(nameof(State), value);
    }

    public Address Owner
    {
        get => PersistentState.GetAddress(nameof(Owner));
        private set => PersistentState.SetAddress(nameof(Owner), value);
    }

    public Address HospitalAddress
    {
        get => PersistentState.GetAddress(nameof(HospitalAddress));
        private set => PersistentState.SetAddress(nameof(HospitalAddress), value);
    }

    public string Name
    {
        get => PersistentState.GetString(nameof(Name));
        private set => PersistentState.SetString(nameof(Name), value);
    }

    public string Email
    {
        get => PersistentState.GetString(nameof(Email));
        private set => PersistentState.SetString(nameof(Email), value);
    }

    public bool HospitalHandleRegister(uint state)
    {
        Assert(Message.Sender == this.HospitalAddress);
        Log(new HospitalHandleRegisterLog{
            state = state,
            doctorAddress = this.Owner,
            hospitalAddress = Message.Sender
        });
        this.State = state;
        return true;
    }

    public bool ExaminePatient(Address patientAddress, string report)
    {
        Assert(Message.Sender == this.Owner);
        Assert(this.State == 1);
        Log(new ExaminePatientLog{
            patientAddress = patientAddress,
            doctorAddress = this.Owner,
            hospitalAddress = this.HospitalAddress,
            report = report;
        });
        return true;
    }

    public struct HospitalHandleRegisterLog
    {
        [Index]
        public uint state;
        [Index]
        public Address doctorAddress;
        [Index]
        public Address hospitalAddress;
    }

    public struct ExaminePatientLog
    {
        [Index]
        public Address patientAddress;
        [Index]
        public Address doctorAddress;
        [Index]
        public Address hospitalAddress;
        public string report;
    }

}