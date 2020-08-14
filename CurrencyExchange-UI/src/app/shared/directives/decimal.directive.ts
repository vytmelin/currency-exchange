import { Directive, ElementRef, HostListener } from '@angular/core';

@Directive({
  selector: '[appDecimalDirective]',
})
export class DecimalDirective {
  private regex: RegExp = new RegExp(/^\d+[.,]?\d{0,2}$/g); // user can put . or , char.
  // input also cannot start from , or .
  private specialKeys: Array<string> = [
    'Backspace',
    'Tab',
    'End',
    'Home',
    '-',
    'ArrowLeft',
    'ArrowRight',
    'Del',
    'Delete',
  ];

  constructor(private el: ElementRef) {}

  @HostListener('keydown', ['$event'])
  onKeyDown(event: KeyboardEvent) {
    if (this.specialKeys.includes(event.key)) {
      return;
    }
    const current: string = this.el.nativeElement.value;
    const position = this.el.nativeElement.selectionStart;
    const next: string = [
      current.slice(0, position),
      event.key == 'Decimal' ? '.' : event.key,
      current.slice(position),
    ].join('');
    if (next && !String(next).match(this.regex)) {
      event.preventDefault();
    }
  }

  @HostListener('paste', ['$event'])
  onPaste(event: any) {
    let clipboardData: any;
    let pastedInput: any;

    if (window['clipboardData']) {
      // IE
      clipboardData = window['clipboardData'];
      pastedInput = clipboardData.getData('text');
    } else if (event.clipboardData && event.clipboardData.getData) {
      // other browsers
      clipboardData = event.clipboardData;
      pastedInput = clipboardData.getData('text/plain');
    }

    const regex = new RegExp('^[0-9.,]*$');
    if (!regex.test(pastedInput)) event.preventDefault();
  }
}
