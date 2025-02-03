import { selectModel } from "../../../shares/reusable/common-model/select-model";

export interface ProductPostModel {
    name: string;
    categoryId: number;
    description: string;
    price: number;
    isPublic: boolean;
    specAttributes: SpecAttribute[];
    specInventories: SpecInventory[];
    warehouseId: number;
    totalInventory: number;
}

export interface SpecAttribute {
    specName: string;
    isPrimary: boolean;
    specItems: SpecItem[];
}

export interface SpecItem{
    specValue: string;
    specImage: File | null;
}

export interface SpecInventory{
    primarySpecValueName: string;
    subSpecValueName: string;
    inventory: number;
}

export interface WarehouseItem extends selectModel {
}