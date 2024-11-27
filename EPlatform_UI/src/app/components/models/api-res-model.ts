export interface ApiResModel<T> {
    status: number
    message: string
    data: T
    resources: Resource[]
    timestamp: string
  }
  
  export interface Resource {
    _Link: string
    method: string
  }