import { Component, ElementRef, EventEmitter, forwardRef, Injector, Input, OnInit, Optional, Output, Self, ViewChild } from '@angular/core';
import { QuillConfig, QuillEditorComponent } from 'ngx-quill';
import { DataService } from 'src/app/services/data.service';
import * as QuillNamespace from 'quill';

import Quill from 'quill';
import BlotFormatter from 'quill-blot-formatter';

import { AbstractControl, ControlValueAccessor, FormBuilder, FormControl, FormGroup, NgControl, NG_VALIDATORS, NG_VALUE_ACCESSOR, ValidationErrors, Validator } from '@angular/forms';

Quill.register('modules/blotFormatter', BlotFormatter);
const icons = Quill.import('ui/icons');

@Component({
  selector: 'quill-wrapper',
  templateUrl: './quill-wrapper.component.html',
  styleUrls: ['./quill-wrapper.component.scss'],
  providers: [
    // {
    //   provide: NG_VALUE_ACCESSOR,
    //   useExisting: QuillWrapperComponent,
    //   multi: true
    // },
    // {
    //   provide: NG_VALIDATORS,
    //   useExisting: QuillWrapperComponent,
    //   multi: true
    // }
  ]
})
export class QuillWrapperComponent implements ControlValueAccessor, Validator {
  public modules: any;
  @ViewChild('quillFile') quillFileRef: ElementRef;
  @ViewChild(QuillEditorComponent, { static: true }) editor: QuillEditorComponent;

  onChange = (value: string) => { };
  onTouched = () => { };
  touched = false;
  @Input() disabled = false;

  private quillFile: any;
  private quillRef: any;

  quillForm: FormGroup;

  // @Input()
  // get value(): string {
  //   return this.quillForm.value;  
  // }
  // set value(text: string) {
  //   console.log(text);
  //   this.quillForm.get('text')?.setValue(text);
  // }

  constructor(
    private dataService: DataService,
    private fb: FormBuilder,
    @Self() @Optional() public control: NgControl) {
    this.control && (this.control.valueAccessor = this);


    this.quillForm = fb.group({
      text: ['']
    })

    icons.bold = null;
    icons.italic = null;
    icons['blockquote'] = null;
    icons['code-block'] = null;
    icons.list = null;
    icons.indent = null;
    icons.link = null;
    icons.image = null;
    this.modules = {
      blotFormatter: {},
      toolbar: {
        container: [
          ['bold', 'italic'],
          ['blockquote', 'code-block'],
          [{ 'list': 'ordered' }, { 'list': 'bullet' }],
          [{ 'indent': '-1' }, { 'indent': '+1' }],
          ['link', 'image'],
        ],
        handlers: {
          image: (image: any) => this.imageHandler(image),
        },
      },
    };
  }

  validate(control: AbstractControl): ValidationErrors | null {
    if (control.invalid) {
      return {
        errors: true
      }
    }
    return null;
  }

  getErrors() {
    return this.control.invalid ? { ivalid: true } : null;
  }

  ///metode interfejsa
  writeValue(obj: string): void {
    this.quillForm.setValue({ text: obj });
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  setDisabledState(disabled: boolean) {
    this.disabled = disabled;
  }
  ////
  imageHandler(image: any) {
    this.quillFileRef.nativeElement.click();
  }

  getEditorInstance(instance: any) {
    this.quillRef = instance;
    console.log(this.quillRef);
  }

  fileSelected(ev: any) {
    this.quillFile = ev.target.files[0];

    this.dataService.uploadImage(this.quillFile).subscribe((path: any) => {
      const index = this.editor.quillEditor.getSelection()?.index;
      this.editor.quillEditor.insertEmbed(index ? index : 0, 'image', path);
    });
  }

  public get invalid(): boolean {
    return (this.control ? this.control.invalid : false) as boolean;
  }

  public get showError(): boolean {
    if (!this.control) {
      return false;
    }

    const { dirty, touched } = this.control;

    return (this.invalid ? (dirty || touched) : false) as boolean;
  }

  // public getErrors() {
  //   if (!this.control) {
  //     return false;
  //   }

  //   const { errors } = this.control;
  //   console.log(errors);
  //   return true;
  //   //return Object.keys(errors).map(key => { console.log(key) });
  // }

  onQuillContentChanged(event: any) {
    this.onChange(this.quillForm.value?.text);
  }

  onQuillTouched() {
    if (!this.touched) {
      this.touched = true;
      this.onTouched();
    }
  }
}
