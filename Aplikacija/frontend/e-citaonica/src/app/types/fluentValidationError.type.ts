export type FluentValidationError = {
    error: {
        errors: {
            [field: string]: string
        }
    }
}