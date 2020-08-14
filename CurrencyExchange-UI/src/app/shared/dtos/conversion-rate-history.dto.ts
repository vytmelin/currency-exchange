import { DailyConversionRate } from '../models/daily-conversion-rate.model';

export class ConversionRateHistoryDto {
  constructor(
    public fromCurrency: string,
    public toCurrency: string,
    public dailyConversionRates: Array<DailyConversionRate>,
    public errorCode: number
  ) {}
}
