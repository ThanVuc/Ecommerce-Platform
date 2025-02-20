import { SpecAttribute, SpecInventory } from "./product-update-response"

export interface ProductDetailResponse{
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
        createdAt: string,
        updatedAt: string
}