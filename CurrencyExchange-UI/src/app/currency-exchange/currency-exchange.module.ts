import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CurrencyExchangeComponent } from './currency-exchange.component';
import { SharedModule } from '../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@NgModule({
  declarations: [CurrencyExchangeComponent],
  imports: [CommonModule, SharedModule, FormsModule, ReactiveFormsModule],
})
export class CurrencyExchangeModule {}
