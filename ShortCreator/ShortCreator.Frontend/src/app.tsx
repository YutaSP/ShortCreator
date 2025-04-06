import { BrowserRouter as Router } from "react-router-dom"
import NavMenuMain from "./components/nav"
import MainRoutes from "./routes"
import GlobalErrorBounary from "./errorBoundaries/globalErrorBoundary"


const App : React.FC = () => {
    return(
        <GlobalErrorBounary>
            <Router>
                <NavMenuMain />
                <MainRoutes />
            </Router>
        </GlobalErrorBounary>
    )
}

export default App