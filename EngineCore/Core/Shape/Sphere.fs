﻿module Engine.Core.Shapes.Sphere
open Engine.Core.Interfaces.IHitable
open Engine.Core.Interfaces.IMaterial
open Engine.Core.Point
open Engine.Core.Ray

type Sphere =
    struct
        val center: Point
        val radius: float
        val material: IMaterial
        new(c:Point, r:float, mate) = {center = c; radius = r; material = mate}
        member this.ShadowHit(ray:Ray) = (this:>IHitable).ShadowHit(ray)
        member this.Hit(r:Ray, tMin:float, tMax:float) = (this:>IHitable).Hit(r,tMin,tMax)
        interface IHitable with
            member this.ShadowHit(ray:Ray) =
                let bHit, record = this.Hit(ray, 0.00001, 99999999.)
                if bHit then
                    bHit, record.t
                else
                    false, 0.0
            member this.Hit(r:Ray, tMin:float, tMax:float) =
                let oc = r.Origin() - this.center
                let a = r.Direction().Dot(r.Direction())
                let b = 2.0 * oc.Dot(r.Direction())
                let c = oc.Dot(oc) - this.radius*this.radius
                let discriminant = b*b-4.0*a*c
                if discriminant > 0 then
                    let tmp = (-b - sqrt(discriminant))/(2.0*a)
                    if tmp < tMax && tmp > tMin then
                        let p = r.PointAtParameter(tmp)
                        (true, HitRecord(true, tmp, p, (p-this.center)/this.radius, r, Some this.material))
                    else
                        let tmp = (-b + sqrt(discriminant))/(2.0*a)
                        if tmp < tMax && tmp > tMin then
                            let p = r.PointAtParameter(tmp)
                            (true, HitRecord(true, tmp, p, (p-this.center)/this.radius, r, Some this.material))
                        else
                            (false, HitRecord.Nothing)
                else
                    (false, HitRecord.Nothing)
    end