import { selectModel } from "../../../shares/reusable/common-model/select-model";

export interface ProductPostModel {
    Name: string;
    CategoryId: number;
    Description: string;
    Price: number;
    IsPublic: boolean;
    SpecAttributes: SpecAttribute[];
    SpecInventories: SpecInventory[];
    WarehouseId: number;
    TotalInventory: number;
    CoverImage: File | null;
}

export interface SpecAttribute {
    SpecName: string;
    IsPrimary: boolean;
    SpecItems: SpecItem[];
}

export interface SpecItem{
    SpecValue: string;
    SpecImage: File | null;
}

export interface SpecInventory{
    PrimarySpecValueName: string;
    SubSpecValueName: string;
    Inventory: number;
}

export interface WarehouseItem extends selectModel {
}