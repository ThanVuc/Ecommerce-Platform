export interface GetPurchaseOrdersModel {
    orderId: number
    orderStatus: string
    createAt: string
    shopAvt: any
    shopName: any
    paymentName: string
    products: Product[]
  }
  
  export interface Product {
    productId: string
    productAvtImg: string
    productName: string
    quantity: number
    price: number
  }