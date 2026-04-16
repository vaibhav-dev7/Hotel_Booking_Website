export interface CouponResponse {
  isValid: boolean;
  code: string;
  discountPercentage: number;
}

export interface CreateCoupon {
  code: string;
  discountPercentage: number;
  expiryDate: string;
}
