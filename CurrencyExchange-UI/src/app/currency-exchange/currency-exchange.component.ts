import { Component, OnInit } from '@angular/core';
import { CurrencyExchangeService } from '../shared/services/currency-exchange.service';
import { MatSelectChange } from '@angular/material/select';
import { ConversionRateDto } from '../shared/dtos/conversion-rate.dto';
import * as CanvasJS from '../../assets/canvasjs.min.js';
import { ConversionRateHistoryDto } from '../shared/dtos/conversion-rate-history.dto';

@Component({
  selector: 'app-currency-exchange',
  templateUrl: './currency-exchange.component.html',
  styleUrls: ['./currency-exchange.component.scss'],
})
export class CurrencyExchangeComponent implements OnInit {
  public currencies: Array<string>;
  public dates: Array<string>;
  public selectedFromCurrency: string;
  public selectedToCurrency: string;
  public selectedDate: string;
  public amountFromInput: number;
  public amountTo: string;
  public conversionRate: number;
  public convertedAmount: number;
  public errorCode: number;

  constructor(private currencyExchangeService: CurrencyExchangeService) {}

  public ngOnInit(): void {
    this.currencyExchangeService
      .getCurrencies()
      .subscribe((currencies: Array<string>) => {
        this.currencies = currencies;
      });
    this.currencyExchangeService
      .getDates()
      .subscribe((dates: Array<string>) => {
        this.selectedDate = dates[0];
        this.dates = dates;
      });
  }

  public onConvertClick(): void {
    if (this.amountFromInput && this.conversionRate)
      this.amountTo = (this.amountFromInput * this.conversionRate).toFixed(5);
  }

  public isConvertDisabled(): boolean {
    return (
      !this.selectedDate ||
      !this.selectedFromCurrency ||
      !this.selectedToCurrency ||
      !this.amountFromInput
    );
  }

  public isHistoryDisabled(): boolean {
    return !this.selectedFromCurrency || !this.selectedToCurrency;
  }

  public getErrorMessage(): string {
    let message = '';

    switch (this.errorCode) {
      case 1:
        message = 'Rate for selected currencies not found.';
        break;
      case 2:
        message = 'Currency not provided';
        break;
      default:
        break;
    }

    return message;
  }

  public onHistoryClick(): void {
    this.currencyExchangeService
      .getConversionRateHistory(
        this.selectedFromCurrency,
        this.selectedToCurrency
      )
      .subscribe((dto: ConversionRateHistoryDto) => {
        this.buildChart(dto);
      });
  }

  public onDropdownChange(event: MatSelectChange): void {
    if (
      this.selectedDate &&
      this.selectedFromCurrency &&
      this.selectedToCurrency
    ) {
      this.currencyExchangeService
        .getConversionRate(
          this.selectedFromCurrency,
          this.selectedToCurrency,
          this.selectedDate
        )
        .subscribe((result: ConversionRateDto) => {
          this.conversionRate = result.conversionRate;
          this.errorCode = result.errorCode;
        });
    } else {
      this.conversionRate = null;
    }
    this.amountTo = null;
  }

  private buildChart(dto: ConversionRateHistoryDto): void {
    let dataPoints = dto.dailyConversionRates.map((rate) => {
      return {
        x: new Date(rate.date),
        y: rate.rate,
      };
    });
    let chart = new CanvasJS.Chart('chartContainer', {
      zoomEnabled: true,
      animationEnabled: true,
      exportEnabled: true,
      title: {
        text:
          dto.fromCurrency +
          ' to ' +
          dto.toCurrency +
          ' conversion rate history',
      },
      data: [
        {
          type: 'line',
          dataPoints: dataPoints,
        },
      ],
    });

    chart.render();
  }
}
