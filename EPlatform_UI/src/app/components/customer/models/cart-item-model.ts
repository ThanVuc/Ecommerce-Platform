export interface CartItemModel {
    cartItemId: number
    productName: string
    productPrice: number
    availableQuantity: number
    quantity: number
    productAvtImg: string
    productId: number
    specInfo?: string
    isSelected: boolean
    shopId: string
    shopName: string
    shopLogoUrl: string
  }