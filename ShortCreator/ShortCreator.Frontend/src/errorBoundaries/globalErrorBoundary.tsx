import React, { ReactNode } from "react";
import { ErrorBoundary, FallbackProps } from "react-error-boundary";



const GlobalErrorBounary : React.FC<{ children: ReactNode }> = ({children}) => {
    const FallbackComponent: React.FC<FallbackProps> = ({ error, resetErrorBoundary }) => {
        //console log to be repalced with call to log entry
        console.log(error.message)

        return (
            <div>
              <h1>Something went wrong!</h1>
              <p>{error.message}</p>
              <button onClick={resetErrorBoundary}>Try Again</button>
            </div>
        )
    }

    return (
        <ErrorBoundary FallbackComponent={FallbackComponent}>
            {children}
        </ErrorBoundary>
    )
}

export default GlobalErrorBounary