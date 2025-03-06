export interface CreateOrderModel {
    email: string
    phone: string
    address: string
    cartItems: CartItem[]
}

export interface CartItem {
    productId: string
    quantity: number
    specInfo: string
}