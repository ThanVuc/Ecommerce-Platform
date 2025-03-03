import { SpecAttribute, SpecInventory } from "../../shopowner/models/product-update-response"


export interface ProductDetailModel {
    name: string
    price: number
    description: string
    avtImageUrl: string
    sold: string
    availabel: number
    specAttributes: SpecAttribute[]
    specInventories: SpecInventory[]
    categories: CategoryOfProduct[]
    shopId: string
    shopName: string
    logoUrl: string
  }

  export interface CategoryOfProduct {
    categoryId: number;
    name: string;
    slug: string;
  }