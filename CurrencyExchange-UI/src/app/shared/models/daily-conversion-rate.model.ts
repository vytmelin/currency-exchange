export class DailyConversionRate {
  constructor(
    public date: string,
    public rate: number,
    public errorCode: number
  ) {}
}
