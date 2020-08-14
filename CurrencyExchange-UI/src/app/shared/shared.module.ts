import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatSelectModule } from '@angular/material/select';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatInputModule } from '@angular/material/input';
import { DecimalDirective } from './directives/decimal.directive';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';

export const materialModules = [
  MatSelectModule,
  MatToolbarModule,
  MatInputModule,
  MatButtonModule,
  MatFormFieldModule,
];

@NgModule({
  declarations: [DecimalDirective],
  imports: [...materialModules, CommonModule],
  exports: [...materialModules, DecimalDirective],
})
export class SharedModule {}
