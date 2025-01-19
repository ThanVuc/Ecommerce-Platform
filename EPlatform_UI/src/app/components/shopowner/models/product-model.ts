export interface ProductModel {
    productId: number
    avtImgUrl: any
    price: number
    isPublic: boolean
    name: string
    inventory: Inventory
    slug: string
}

export interface Inventory {
    availableQuantity: any
    isAvailable: boolean
}
