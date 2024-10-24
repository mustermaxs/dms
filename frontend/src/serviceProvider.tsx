import { Type } from "typescript";

export class ServiceLocator {
    private static instance: ServiceLocator;
    private serviceMap: Map<string, any>;
    private constructor() {
        this.serviceMap = new Map<string, any>();
    }

    public static getInstance(): ServiceLocator {
        if (!ServiceLocator.instance) {
            ServiceLocator.instance = new ServiceLocator();
        }
        return ServiceLocator.instance;
    }

    public static register<TServiceInterface, TServiceImpl extends TServiceInterface>(
        serviceName: string, 
        serviceImplementation: new (...args: any[]) => TServiceImpl
    ) {
        const instance = this.getInstance();
        instance.serviceMap.set(serviceName, serviceImplementation);
    }

    
    public static resolve<TServiceInterface>(
        serviceName: string 
    ): TServiceInterface {
        const instance = this.getInstance();
        const serviceConstructor = instance.serviceMap.get(serviceName);

        if (!serviceConstructor) {
            throw new Error(`Service ${serviceName} not registered.`);
        }

        return new serviceConstructor();
    }
}
