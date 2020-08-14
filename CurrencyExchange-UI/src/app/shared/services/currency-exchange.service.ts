import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ConversionRateDto } from '../dtos/conversion-rate.dto';
import { ConversionRateHistoryDto } from '../dtos/conversion-rate-history.dto';

@Injectable({
  providedIn: 'root',
})
export class CurrencyExchangeService {
  constructor(private http: HttpClient) {}

  public getCurrencies(): Observable<Array<string>> {
    return this.http.get<Array<string>>(
      environment.apiUrl + 'exchange-rates/currencies'
    );
  }

  public getDates(): Observable<Array<string>> {
    return this.http.get<Array<string>>(
      environment.apiUrl + 'exchange-rates/dates'
    );
  }

  public getConversionRate(
    from: string,
    to: string,
    date: string
  ): Observable<ConversionRateDto> {
    return this.http.get<ConversionRateDto>(
      environment.apiUrl +
        'exchange-rates/conversion-rate?fromCurrency=' +
        from +
        '&toCurrency=' +
        to +
        '&date=' +
        date
    );
  }

  public getConversionRateHistory(
    from: string,
    to: string
  ): Observable<ConversionRateHistoryDto> {
    return this.http.get<ConversionRateHistoryDto>(
      environment.apiUrl +
        'exchange-rates/historic-data?fromCurrency=' +
        from +
        '&toCurrency=' +
        to
    );
  }
}
