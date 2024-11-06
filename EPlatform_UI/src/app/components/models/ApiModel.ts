export interface ApiModel<T> {
    status: number
    message: string
    data: T
    timestamp: string
  }