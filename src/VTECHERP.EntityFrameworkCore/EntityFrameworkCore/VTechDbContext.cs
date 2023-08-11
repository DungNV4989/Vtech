using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.MultiTenancy;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using VTECHERP.Entities;

namespace VTECHERP.EntityFrameworkCore;

[ConnectionStringName("Default")]
public class VTechDbContext :
    AbpDbContext<VTechDbContext>,
    IIdentityDbContext
{
    #region Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    #endregion
    #region Permission
    public DbSet<PermissionModule> PermissionModules { get; set; }
    public DbSet<PermissionGroup> PermissionGroups { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    #endregion
    #region Business
    public DbSet<Entry> Entries { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<EntryAccount> EntryAccounts { get; set; }
    public DbSet<EntryLog> EntryLogs { get; set; }
    public DbSet<Debt> Debts { get; set; }
    public DbSet<DebtReport> DebtReports { get; set; }
    public DbSet<Attachment> Attachments { get; set; }
    public DbSet<ProductCategories> ProductCategories { get; set; }
    public DbSet<Suppliers> Suppliers { get; set; }
    public DbSet<Products> Products { get; set; }
    public DbSet<SaleOrderLineConfirm> SaleOrderLineConfirms { get; set; }
    public DbSet<SaleOrderLines> SaleOrderLines { get; set; }
    public DbSet<SaleOrders> SaleOrders { get; set; }
    public DbSet<Stores> Stores { get; set; }
    public DbSet<Provinces> Provinces { get; set; }
    public DbSet<Districts> Districts { get; set; }
    public DbSet<Wards> Wards { get; set; }
    public DbSet<BatchStatus> BatchStatus { get; set; }
    public DbSet<StoreProduct> StoreProducts { get; set; }
    public DbSet<WarehousingBill> WarehousingBills { get; set; }
    public DbSet<WarehousingBillProduct> WarehousingBillProducts { get; set; }
    public DbSet<WarehouseTransferBill> WarehouseTransferBills { get; set; }
    public DbSet<WarehouseTransferBillProduct> WarehouseTransferBillProducts { get; set; }
    public DbSet<WarehousingBillLogs> WarehousingBillLogs { get; set; }
    public DbSet<PaymentReceipt> PaymentReceipts { get; set; }
    public DbSet<OrderTransport> OrderTransports { get; set; }
    public DbSet<OrderTransportSale> OrderTransportSales { get; set; }
    public DbSet<UserStore> UserStores { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<CustomerSupport> CustomerSupports { get; set; }
    public DbSet<SupplierOrderReport> SuppliersOrderReports { get; set; }
    public DbSet<DraftTicket> DraftTickets { get; set; }
    public DbSet<DraftTicketProduct> DraftTicketProducts { get; set; }
    public DbSet<BillCustomer> BillCustomers { get; set; }
    public DbSet<BillCustomerProduct> BillCustomerProducts { get; set; }
    public DbSet<BillCustomerProductBonus> BillCustomerProductBonus { get; set; }
    public DbSet<PriceTable> PriceTables { get; set; }
    public DbSet<PriceTableCustomer> PriceTableCustomer { get; set; }
    public DbSet<PriceTableStore> PriceTableStore { get; set; }
    public DbSet<PriceTableProduct> PriceTableProducts { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<TransportInformation> TransportInfomations { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<CustomerReturn> CustomerReturns {  get; set; }
    public DbSet<CustomerReturnProduct> CustomerReturnProducts { get; set; }
    public DbSet<DebtReminderLog> DebtReminderLogs { get; set; }
    public DbSet<BillCustomerLog> CustomerBillLogs { get; set; }
    public DbSet<TransportInformationLog> TransportInformationLogs { get; set; }
    public DbSet<HistoryChangeCostPriceProduct> HistoryChangeCostPriceProducts { get; set; }
    public DbSet<HistoryPrintBillCustomer> HistoryPrintBillCustomers { get; set; }
    public DbSet<TransporstBills> TransporstBills { get; set; }
    public DbSet<GroupTransportInformation> GroupTransportInformation { get; set; }

    public DbSet<ProductLog> ProductLogs { get; set; }
    public DbSet<Promotion> Promotions { get; set; }
    public DbSet<Voucher> Vouchers { get; set; }
    public DbSet<IntegratedCassoLog> IntegratedCassoLogs { get; set; }
    public DbSet<ProductStockView> ProductStockViews { get; set; }
    public DbSet<ProductView> ProductViews { get; set; }
    public DbSet<BankInfo> BankInfos { get; set; }
    public DbSet<StoreShippingInformation> StoreShippingInformation { get; set; }
    public DbSet<CarrierShippingInformation> CarrierShippingInformation { get; set; }
    public DbSet<CarrierShippingLog> CarrierShippingLogs { get; set; }
    public DbSet<GHTKPostOffice> GHTKPostOffices { get; set; }
    public DbSet<Enterprise> Enterprises { get; set; }
    public DbSet<Agency> Agencies { get; set; }
    public DbSet<DayConfiguration> DayConfigurations { get; set; }
    #endregion
    public VTechDbContext(DbContextOptions<VTechDbContext> options) : base(options)
    {

    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.UseCollation("SQL_Latin1_General_CP1_CI_AI");
        builder.SetMultiTenancySide(MultiTenancySides.Tenant);
        builder.HasSequence<int>("EntryNumbers");

        builder.Entity<Entry>()
            .Property(o => o.Code)
            .HasDefaultValueSql("FORMAT(NEXT VALUE FOR EntryNumbers,'0000000000')");

        builder.HasSequence<int>("ProudctSequenceId");

        builder.Entity<Products>()
            .Property(o => o.SequenceId)
            .HasDefaultValueSql("FORMAT(NEXT VALUE FOR ProudctSequenceId,'0000000000')");

        builder.HasSequence<int>("OrderNumbers");

        builder.Entity<SaleOrders>()
            .Property(o => o.Code)
            .HasDefaultValueSql("FORMAT(NEXT VALUE FOR OrderNumbers,'0000000000')");

        builder.HasSequence<int>("WarehousingBillNumbers");

        builder.Entity<WarehousingBill>()
            .Property(o => o.Code)
            .HasDefaultValueSql("FORMAT(NEXT VALUE FOR WarehousingBillNumbers,'0000000000')");

        builder.HasSequence<int>("WarehouseTransferBillNumbers");

        builder.Entity<WarehouseTransferBill>()
            .Property(o => o.Code)
            .HasDefaultValueSql("FORMAT(NEXT VALUE FOR WarehouseTransferBillNumbers,'0000000000')");

        builder.HasSequence<int>("PaymentReceiptNumbers");

        builder.Entity<PaymentReceipt>()
            .Property(o => o.Code)
            .HasDefaultValueSql("FORMAT(NEXT VALUE FOR PaymentReceiptNumbers,'0000000000')");

        builder.HasSequence<int>("SaleOrderLineNumbers");

        builder.Entity<SaleOrderLines>()
        .Property(o => o.Code)
        .HasDefaultValueSql("FORMAT(NEXT VALUE FOR SaleOrderLineNumbers,'0000000000')");

        builder.HasSequence<int>("EntryAccountNumbers");

        builder.Entity<EntryAccount>()
        .Property(o => o.Code)
        .HasDefaultValueSql("FORMAT(NEXT VALUE FOR EntryAccountNumbers,'0000000000')");

        builder.HasSequence<int>("EntryLogNumbers");

        builder.Entity<EntryLog>()
        .Property(o => o.Code)
        .HasDefaultValueSql("FORMAT(NEXT VALUE FOR EntryLogNumbers,'0000000000')");

        builder.HasSequence<int>("DraftTicketNumbers");

        builder.Entity<DraftTicket>()
        .Property(o => o.Code)
        .HasDefaultValueSql("FORMAT(NEXT VALUE FOR DraftTicketNumbers,'0000000000')");

        builder.Entity<Customer>()
            .Property(p => p.DateOfBirth).HasColumnType("Date");

        builder.HasSequence<int>("BillCustomerCode");

        builder.Entity<BillCustomer>()
            .Property(o => o.Code)
            .HasDefaultValueSql("FORMAT(NEXT VALUE FOR BillCustomerCode,'0000000000')");

        builder.HasSequence<int>("BillCustomerProductCode");

        builder.Entity<BillCustomerProduct>()
            .Property(o => o.Code)
            .HasDefaultValueSql("FORMAT(NEXT VALUE FOR BillCustomerProductCode,'0000000000')");

        builder.HasSequence<int>("BillCustomerProductBonusCode");
        builder.Entity<TransportInformation>()
            .Property(o => o.Code)
            .HasDefaultValueSql("FORMAT(NEXT VALUE FOR TransportInformationCode,'0000000000')");

        builder.HasSequence<int>("TransportInformationCode");

        builder.Entity<Employee>()
            .Property(o => o.Code)
            .HasDefaultValueSql("FORMAT(NEXT VALUE FOR EmployeeCode,'0000000000')");

        builder.HasSequence<int>("EmployeeCode");

        builder.Entity<BillCustomerProductBonus>()
            .Property(o => o.Code)
            .HasDefaultValueSql("FORMAT(NEXT VALUE FOR BillCustomerProductBonusCode,'0000000000')");

        builder.HasSequence<int>("PriceChartNumbers");
        builder.Entity<PriceTable>().Property(p => p.Code).HasDefaultValueSql("FORMAT(NEXT VALUE FOR PriceChartNumbers,'0000000000')");


        builder.HasSequence<int>("DebtReminderLogCode");

        builder.Entity<DebtReminderLog>()
            .Property(o => o.Code)
            .HasDefaultValueSql("FORMAT(NEXT VALUE FOR DebtReminderLogCode,'0000000000')");


        builder.HasSequence<int>("CustomerCodes");
        builder.Entity<Customer>().Property(p => p.Code).HasDefaultValueSql("FORMAT(NEXT VALUE FOR CustomerCodes,'0000000000')");

        builder.HasSequence<int>("ProductLogs");
        builder.Entity<ProductLog>().Property(p => p.Code).HasDefaultValueSql("FORMAT(NEXT VALUE FOR ProductLogs,'0000000000')");

        builder.HasSequence<int>("CustomerReturn");
        builder.Entity<CustomerReturn>()
            .Property(x => x.Code)
            .HasDefaultValueSql("FORMAT(NEXT VALUE FOR CustomerReturnCode,'0000000000')");

        builder.Entity<ProductStockView>()
            .ToView("ProductStockView")
            .HasKey(t => t.Id);
        builder.Entity<ProductView>()
            .ToView("ProductView")
            .HasKey(t => t.Id);

        //builder.HasSequence<int>("IdentityUserCode");
        //builder.Entity<IdentityUser>()
        //   .Property("UserCode")
        //   .HasDefaultValueSql("FORMAT(NEXT VALUE FOR IdentityUserCode,'0000000000')");

        builder.HasSequence<int>("ProductCategoryNumbers");
        builder.Entity<ProductCategories>()
        .Property(o => o.Code)
        .HasDefaultValueSql("CONCAT('DMSP-',FORMAT(NEXT VALUE FOR ProductCategoryNumbers,'0000000000'))");

        builder.HasSequence<int>("StoreNumbers");
        builder.Entity<Stores>()
        .Property(o => o.Code)
        .HasDefaultValueSql("CONCAT('CH-',FORMAT(NEXT VALUE FOR StoreNumbers,'0000000000'))");

        builder.HasSequence<int>("SupplierNumbers");
        builder.Entity<Suppliers>()
        .Property(o => o.Squence)
        .HasDefaultValueSql("CONCAT('NCC-',FORMAT(NEXT VALUE FOR SupplierNumbers,'0000000000'))");

        builder.ConfigurePermissionManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentity();
        builder.ConfigureSettingManagement();
        builder.ConfigureOpenIddict();
    }
}
