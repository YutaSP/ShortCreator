import { Route, Routes, RoutesProps } from 'react-router-dom';
import Management from './pages/management';
import Landing from './pages/landing';

const MainRoutes : React.FC = () => {
    return(
        <Routes>
            <Route path="/" element={<Landing />} />
            <Route path="/management" element={<Management />} />
        </Routes>
    )
}

export default MainRoutes