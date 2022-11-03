using OrderCloud.Catalyst;
using OrderCloud.SDK;

namespace Customer.OrderCloud.Common.Models
{
	//    **************************************************************************************
	//    *  XpBoilerplate.cs
	//    *  
	//    *  Stores boilerplate code for wiring up strongly typed xp. Keeps it out of sight.
	//    *  Under most OrderCloud marketplace set-ups, these XP patterns should work unchanged.
	//    *  
	//    **************************************************************************************



	//    **************************************************************************************
	//    *  Full models with strongly typed Xp. Use these as type arguments in the OC SDK.
	//    **************************************************************************************
	#region Full Models
	public class AddressWithXp : Address<AddressXp> { }
	public class BuyerAddressWithXp : BuyerAddress<AddressXp> { }
	public class ShipFromAddressWithXp : Address<ShipFromAddressXp> { }
	public class InventoryRecordWithXp : InventoryRecord<InventoryRecordXp, ShipFromAddressWithXp> { }
	public class ShipmentWithXp : Shipment<ShipmentXp, ShipFromAddressWithXp, AddressWithXp> { }
	public class LineItemWithXp : LineItem<LineItemXp, LineItemProductWithXp, LineItemVariantWithXp, AddressWithXp, ShipFromAddressWithXp> { }
	public class ExtendedLineItemWithXp : ExtendedLineItem<LineItemXp, OrderWithXp, LineItemProductWithXp, LineItemVariantWithXp, AddressWithXp, ShipFromAddressWithXp> { }
	public class OrderWithXp : Order<OrderXp, UserWithXp, AddressWithXp> { }
	public class ExtendedOrderWithXp : ExtendedOrder<OrderXp, LineItemWithXp, UserWithXp, AddressWithXp> { }
	public class SupplierOrderWithXp : Order<OrderXp, AdminUserWithXp, ShipFromAddressWithXp> { }
	public class SupplierExtendedOrderWithXp : ExtendedOrder<OrderXp, LineItemWithXp, AdminUserWithXp, ShipFromAddressWithXp> { }
	public class OrderWorksheetWithXp : OrderWorksheet<OrderWithXp, LineItemWithXp, OrderPromotionWithXp, ShipEstimateResponseWithXp, OrderCalculateResponseWithXp, OrderSubmitResponseWithXp, OrderSubmitForApprovalResponseWithXp, OrderApprovedResponseWithXp> { }
	public class SupplierOrderWorksheetWithXp : OrderWorksheet<SupplierOrderWithXp, LineItemWithXp, OrderPromotionWithXp, ShipEstimateResponseWithXp, OrderCalculateResponseWithXp, OrderSubmitResponseWithXp, OrderSubmitForApprovalResponseWithXp, OrderApprovedResponseWithXp> { }
	public class OrderSplitResultWithXp : OrderSplitResult<SupplierOrderWithXp> { }
	public class UserGroupWithXp : UserGroup<UserGroupXp> { }
	public class SupplierUserGroupWithXp : UserGroup<SupplierUserGroupXp> { }
	public class AdminUserGroupWithXp : UserGroup<AdminUserGroupXp> { }
	public class MeUserWithXp : MeUser<UserXp> { }
	public class UserWithXp : User<UserXp> { }
	public class SupplierUserWithXp : User<SupplierUserXp> { }
	public class AdminUserWithXp : User<AdminUserXp> { }
	public class AdHocProductWithXp : AdHocProduct<ProductXp> { }
	public class BuyerProductWithXp : BuyerProduct<ProductXp, PriceScheduleWithXp> { }
	public class LineItemProductWithXp : LineItemProduct<ProductXp> { }
	public class ProductWithXp : Product<ProductXp> { }
	public class ApprovalRuleWithXp : ApprovalRule<ApprovalRuleXp> { }
	public class SellerApprovalRuleWithXp : SellerApprovalRule<SellerApprovalRuleXp> { }
	public class CreditCardWithXp : CreditCard<CreditCardXp> { }
	public class BuyerCreditCardWithXp : BuyerCreditCard<CreditCardXp> { }
	public class LineItemOverrideWithXp : LineItemOverride<AdHocProductWithXp> { }
	public class VariantWithXp : Variant<VariantXp> { }
	public class LineItemVariantWithXp : LineItemVariant<VariantXp> { }
	public class OrderApprovalWithXp : OrderApproval<UserWithXp> { }
	public class PromotionWithXp : Promotion<PromotionXp> { }
	public class OrderPromotionWithXp : OrderPromotion<PromotionXp> { }
	public class OrderReturnWithXp : OrderReturn<OrderReturnXp> { }
	public class SupplierOrderReturnApprovalWithXp : OrderReturnApproval<SupplierUserWithXp> { }
	public class AdminOrderReturnApprovalWithXp : OrderReturnApproval<AdminUserWithXp> { }
	public class SupplierWithXp : Supplier<SupplierXp> { }
	public class ProductSupplierWithXp : ProductSupplier<SupplierXp> { }
	public class ProductCollectionWithXp : ProductCollection<ProductCollectionXp> { }
	public class ShipEstimateWithXp : ShipEstimate<ShipEstimateXp, ShipMethodWithXp> { }
	public class ShipEstimateResponseWithXp : ShipEstimateResponse<ShipEstimateResponseXp, ShipEstimateWithXp> { }
	public class ShipMethodWithXp : ShipMethod<ShipMethodXp> { }
	public class OrderCalculateResponseWithXp : OrderCalculateResponse<OrderCalculateResponseXp, LineItemOverrideWithXp> { }
	public class ShipmentItemWithXp : ShipmentItem<ShipmentItemXp, LineItemProductWithXp, LineItemVariantWithXp> { }
	public class SpecWithXp : Spec<SpecXp, SpecOptionWithXp> { }
	public class SpecOptionWithXp : SpecOption<SpecOptionXp> { }
	public class PaymentWithXp : Payment<PaymentXp, PaymentTransactionWithXp> { }
	public class PaymentTransactionWithXp : PaymentTransaction<PaymentTransactionXp> { }
	public class SpendingAccountWithXp : SpendingAccount<SpendingAccountXp> { }
	public class PriceScheduleWithXp : PriceSchedule<PriceScheduleXp> { }
	public class ProductFacetWithXp : ProductFacet<ProductFacetXp> { }
	public class MessageSenderWithXp : MessageSender<MessageSenderXp> { }
	public class ApiClientWithXp : ApiClient<ApiClientXp> { }
	public class BuyerWithXp : Buyer<BuyerXp> { }
	public class CatalogWithXp : Catalog<CatalogXp> { }
	public class CategoryWithXp : Category<CategoryXp> { }
	public class CostCenterWithXp : CostCenter<CostCenterXp> { }
	public class OrderSubmitResponseWithXp : OrderSubmitResponse<OrderSubmitResponseXp> { }
	public class OrderApprovedResponseWithXp : OrderApprovedResponse<OrderSubmitResponseXp> { }
	public class OrderSubmitForApprovalResponseWithXp : OrderSubmitForApprovalResponse<OrderSubmitResponseXp> { }
	public class OrderCheckoutIEPayloadWithXp : OrderCheckoutIEPayload<IntegrationEventConfigData, OrderWorksheetWithXp> { }
	public class AddToCartIEPayloadWithXp : AddToCartIEPayload<IntegrationEventConfigData, UserWithXp> { }
	public class OpenIDConnectIEPayloadWithXp : OpenIDConnectIEPayload<IntegrationEventConfigData, UserWithXp> { }
	public class OrderReturnIEPayloadWithXp : OrderReturnIEPayload<OrderReturnWithXp, OrderWorksheetWithXp> { }
	public class AddToCartResponseWithXp : AddToCartResponse<AdHocProductWithXp> { }
	#endregion



	//    **************************************************************************************
	//    *   Xp partial classes. Do not define your Xp here, define them elsewhere in partial classes.
	//    **************************************************************************************
	#region Xp Classes
	public partial class IntegrationEventConfigData { }
	public partial class AddressXp { }
	public partial class ShipFromAddressXp { }
	public partial class InventoryRecordXp { }
	public partial class ShipmentXp { }
	public partial class LineItemXp { }
	public partial class OrderXp { }
	public partial class UserGroupXp { }
	public partial class SupplierUserGroupXp { }
	public partial class AdminUserGroupXp { }
	public partial class UserXp { }
	public partial class SupplierUserXp { }
	public partial class AdminUserXp { }
	public partial class ProductXp { }
	public partial class ApprovalRuleXp { }
	public partial class SellerApprovalRuleXp { }
	public partial class CreditCardXp { }
	public partial class VariantXp { }
	public partial class PromotionXp { }
	public partial class OrderReturnXp { }
	public partial class SupplierXp { }
	public partial class ProductCollectionXp { }
	public partial class ShipEstimateXp { }
	public partial class ShipEstimateResponseXp { }
	public partial class ShipMethodXp { }
	public partial class OrderCalculateResponseXp { }
	public partial class ShipmentItemXp { }
	public partial class SpecXp { }
	public partial class SpecOptionXp { }
	public partial class PaymentXp { }
	public partial class PaymentTransactionXp { }
	public partial class SpendingAccountXp { }
	public partial class PriceScheduleXp { }
	public partial class ProductFacetXp { }
	public partial class MessageSenderXp { }
	public partial class ApiClientXp { }
	public partial class BuyerXp { }
	public partial class CatalogXp { }
	public partial class CategoryXp { }
	public partial class CostCenterXp { }
	public partial class OrderSubmitResponseXp { }
	#endregion


}
