export interface ProductUpdateResponse {
    name: string
    slug: string
    categoryId: number
    categoryName: string
    description: string
    price: number
    isPublic: boolean
    specAttributes: SpecAttribute[]
    specInventories: SpecInventory[]
    warehouseId: number
    totalInventory: number
    coverImageUrl: string
}

export interface SpecAttribute {
    specName: string
    isPrimary: boolean
    specItems: SpecItem[]
}

export interface SpecItem {
    specValue: string
    specImageUrl: string
}

export interface SpecInventory {
    primarySpecValueName: string
    subSpecValueName: string
    inventory: number
}
