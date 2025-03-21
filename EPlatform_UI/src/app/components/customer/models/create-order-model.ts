export interface CreateOrderModel {
    email: string
    phone: string
    shippingAddress: string
    cartItems: CartItem[]
  }
  
  export interface CartItem {
    cartItemId: number
    shopId: string
    productId: number
    quantity: number
    price: number
    specInfo: string
  }