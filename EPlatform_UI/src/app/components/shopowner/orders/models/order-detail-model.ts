export interface OrderDetailModel {
    orderId: number
    orderStatus: string
    createAt: string
    accountName: string
    orderNums: number
    email: string
    phone: string
    customerName: string
    shippingAddress: string
    shippingPhone: string
    products: Product[]
  }
  
  export interface Product {
    avtImg: string
    name: string
    quantity: number
    price: number
  }