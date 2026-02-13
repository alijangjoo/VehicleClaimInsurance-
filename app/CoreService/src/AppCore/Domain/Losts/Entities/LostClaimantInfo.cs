namespace Insurance.AppCore.Domain.Parvandes.Entities;

using Insurance.AppCore.Domain.BaseData.CarSystemAndTips.Enums;
using Insurance.AppCore.Domain.BaseData.VehiclePlates.Enums;
using Insurance.AppCore.Domain.BaseData.VehiclePlates.ValueObjects;
using Insurance.AppCore.Domain.Losts.Enums;
using OysterFx.AppCore.Domain.Aggregates;
using System;

public class LostClaimantInfo : Aggregate
{

    /// <summary>
    /// بیمه گر
    /// </summary>
    public Guid InsuranceCompanyBusinessKey { get; set; }
    /// <summary>
    /// شعبه بیمه گر
    /// </summary>
    public Guid InsuranceCompanyBranch { get; set; }
    /// <summary>
    /// اطلاعات مالک/راننده
    /// </summary>
    public PolicyHolderGender Gender { get; set; }
    public string? PolicyHolderFullName { get; set; }

    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? MobileNumber { get; set; }
    /// <summary>
    /// نوع خودرو
    /// </summary>
    public GroupHeadType GroupHeadType { get; set; }
    /// <summary>
    /// سیستم و تیپ خودرو
    /// </summary>
    public Guid CarSystemAndTipBusinessKey { get; set; }
    /// <summary>
    /// نوع پلاک
    /// </summary>
    public VehiclePlateType VehiclePlateType { get; set; }
    /// <summary>
    /// نمونه پلاک
    /// </summary>
    public PlateModelType PlateModelType { get; set; }
    /// <summary>
    /// طرح پلاک
    /// </summary>
    public VehiclePlateDesign VehiclePlateDesign { get; set; }
    /// <summary>
    /// پلاک
    /// </summary>
    public VehiclePlate VehiclePlate { get; set; }
    private LostClaimantInfo() { }
    private LostClaimantInfo(
        Guid insuranceCompanyBusinessKey,
        Guid insuranceCompanyBranch,
        PolicyHolderGender gender,
        string? policyHolderFullName,
        string? fullName,
        string? phoneNumber,
        string? mobileNumber,
        GroupHeadType groupHeadType,
        Guid carSystemAndTipBusinessKey,
        VehiclePlateType vehiclePlateType,
        PlateModelType plateModelType,
        VehiclePlateDesign vehiclePlateDesign,
        VehiclePlate vehiclePlate)
    {
        InsuranceCompanyBusinessKey = insuranceCompanyBusinessKey;
        InsuranceCompanyBranch = insuranceCompanyBranch;
        Gender = gender;
        PolicyHolderFullName = policyHolderFullName;
        FullName = fullName;
        PhoneNumber = phoneNumber;
        MobileNumber = mobileNumber;
        GroupHeadType = groupHeadType;
        CarSystemAndTipBusinessKey = carSystemAndTipBusinessKey;
        VehiclePlateType = vehiclePlateType;
        PlateModelType = plateModelType;
        VehiclePlateDesign = vehiclePlateDesign;
        VehiclePlate = vehiclePlate;
    }

    public static LostClaimantInfo CreateClaimantInfo(
        Guid insuranceCompanyBusinessKey,
        Guid insuranceCompanyBranch,
        PolicyHolderGender gender,
        string? policyHolderFullName,
        string? fullName,
        string? phoneNumber,
        string? mobileNumber,
        GroupHeadType groupHeadType,
        Guid carSystemAndTipBusinessKey,
        VehiclePlateType vehiclePlateType,
        PlateModelType plateModelType,
        VehiclePlateDesign vehiclePlateDesign,
        VehiclePlate vehiclePlate) => new(
            insuranceCompanyBusinessKey,
            insuranceCompanyBranch,
            gender,
            policyHolderFullName,
            fullName,
            phoneNumber,
            mobileNumber,
            groupHeadType,
            carSystemAndTipBusinessKey,
            vehiclePlateType,
            plateModelType,
            vehiclePlateDesign,
            vehiclePlate);
}