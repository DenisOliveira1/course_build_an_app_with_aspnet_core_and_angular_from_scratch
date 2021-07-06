import { Component, Input, Self} from '@angular/core';
import { ControlValueAccessor, NgControl } from '@angular/forms';

@Component({
  selector: 'app-text-input',
  templateUrl: './text-input.component.html',
  styleUrls: ['./text-input.component.css']
})
export class TextInputComponent implements ControlValueAccessor {

  @Input() label: string;
  @Input() type: string = 'text';

  constructor(
    @Self() public ngControl: NgControl
    ) { 
    this.ngControl.valueAccessor = this;
  }

  writeValue(obj: any): void {
  }
  registerOnChange(fn: any): void {
  }
  registerOnTouched(fn: any): void {
  }

  isAVowel(label : string){
    const result =  label.charAt(0).match(/[aeiouy]/i);

    if (result == null) return false;
    return true;
  }

}
